
import React from 'react';

import {
	Col,
	FormGroup,
	Modal,
	ModalHeader,
	ModalBody,
	ModalFooter,
	Button,
	Label,
	Input,
} from 'reactstrap';
import Widget from '../../../components/Widget';

import s from './Solicitacoes.module.scss';

import { Api } from '../../../shared/Api.js'

export default class LojistaSolicitacoes extends React.Component {

	state = {
		loading: false,
		error: '',
		aviso: '',
		solic: { id: 0 }
	};

	componentDidMount() {
		this.loadSolics();
	}

	loadSolics = e => {
		this.setState({ loading: true, error: '', aviso: '', solic: { id: 0 } });
		var api = new Api();
		api.getTokenPortal('lojistaSolicitacao', null).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {
				this.setState({
					loading: false,
					solic: resp.payload
				});
			}
		});
	}

	cancelarSolic = e => {
		e.preventDefault();
		this.setState({ loading: true, error: '', aviso: '' });
		var api = new Api();
		api.postTokenPortal("solicitaVendaCancelamento", JSON.stringify(this.state.solic)).then(resp => {
			if (resp.ok === true) {
				this.setState({
					loading: false,
					aviso: "Venda cancelada com sucesso!",
				});
				this.loadSolics();
			}
			else {
				this.setState({
					loading: false,					
					error: resp.msg
				});
			}
		}).catch(err => {
			this.setState({
				loading: false,				
				error: "Nao foi possivel verificar os dados de sua requisição"
			});
		});
	}

	render() {
		return (
			<div className={s.root}>
				<ol className="breadcrumb">
					<li className="breadcrumb-item">Portal </li>
					<li className="active breadcrumb-item">
						Solicitações pendentes
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				<Modal isOpen={this.state.error.length > 0} toggle={() => this.setState({ error: "" })}>
					<ModalHeader toggle={() => this.setState({ error: "" })}>
						Aviso do Sistema
            		</ModalHeader>
					<ModalBody className="bg-danger-system">
						<div className="modalBodyMain">
							<br />
							{this.state.error}
							<br />
							<br />
						</div>
					</ModalBody>
					<ModalFooter className="bg-white">
						<Button color="primary" onClick={() => this.setState({ error: "" })}> Fechar </Button>
					</ModalFooter>
				</Modal>
				<Modal isOpen={this.state.aviso.length > 0} toggle={() => this.setState({ aviso: "" })}>
					<ModalHeader toggle={() => this.setState({ aviso: "" })}>
						Aviso do Sistema
            		</ModalHeader>
					<ModalBody className="bg-success-system">
						<div className="modalBodyMain">
							<br />
							{this.state.aviso}
							<br />
							<br />
						</div>
					</ModalBody>
					<ModalFooter className="bg-white">
						<Button color="primary" onClick={() => this.setState({ aviso: "" })}> Fechar </Button>
					</ModalFooter>
				</Modal>
				<div align='center'>
					<Button color="primary"
						style={{ width: "200px" }}
						onClick={this.loadSolics}
						disabled={this.state.loading} >
						{this.state.loading === true ? (
							<span className="spinner">
								<i className="fa fa-spinner fa-spin" />
								&nbsp;&nbsp;&nbsp;
											</span>
						) : (
								<div />
							)}
						Atualizar
								</Button>
				</div>
				<br></br>
				{
					this.state.solic.id === 0 ? <div>
						<br></br>
						{
							this.state.loading === true ? <div>
							</div>
								:
								<div>
									<h2>Nenhuma solicitação em aberto</h2>
								</div>
						}
						<br></br>
						<br></br>
					</div>
						:
						<div>
							<Widget>
								<FormGroup row>
									<Label for="normal-field" md={12} className="text-md-left">
										<h5>Cartão </h5>
										<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
											type="text"
											id="fieldName"
											maxLength="50"
											value={this.state.solic.stCartao}
											disabled
										/>
									</Label>
								</FormGroup>
								<FormGroup row>
									<Label for="normal-field" md={12} className="text-md-left">
										<h5>Data </h5>
										<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
											type="text"
											id="fieldName"
											maxLength="50"
											value={this.state.solic.dtSolic}
											disabled
										/>
									</Label>
								</FormGroup>
								<table>
									<tbody>
										<tr>
											<td>
												<FormGroup row>
													<Label for="normal-field" md={4} className="text-md-left">
														<h5>Valor </h5>
													</Label>
													<Col md={7}>
														<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
															type="text"
															id="fieldName"
															maxLength="50"
															value={this.state.solic.stValor}
															disabled
														/>
													</Col>
												</FormGroup>
											</td>
											<td width='20px'></td>
											<td>
												<FormGroup row>
													<Label for="normal-field" md={4} className="text-md-left">
														<h5>Parcelas </h5>
													</Label>
													<Col md={7}>
														<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
															type="text"
															id="fieldName"
															maxLength="50"
															value={this.state.solic.nuParcelas}
															disabled
														/>
													</Col>
												</FormGroup>
											</td>
										</tr>
									</tbody>
								</table>
							</Widget>
							<div align='center'>
								<Button color="danger"
									style={{ width: "200px" }}
									onClick={this.cancelarSolic}
									disabled={this.state.loading} >
									{this.state.loading === true ? (
										<span className="spinner">
											<i className="fa fa-spinner fa-spin" />
											&nbsp;&nbsp;&nbsp;
											</span>
									) : (
											<div />
										)}
									Cancelar Solicitação
								</Button>
							</div>
							<br></br>
						</div>
				}
			</div>
		)
	}
}
