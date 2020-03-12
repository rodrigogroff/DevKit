
import React from 'react';

import {
	Col,
	FormGroup,
	Modal,
	ModalHeader,
	ModalBody,
	ModalFooter,
	InputGroup,
	InputGroupAddon,
	InputGroupText,
	Button,
	Label,
	Input,
} from 'reactstrap';
import Widget from '../../../components/Widget';

import s from './Solicitacoes.module.scss';

import { Api } from '../../../shared/Api.js'

export default class AssociadoSolicitacoes extends React.Component {

	state = {
		loading: false,
		error: '',
		aviso: '',
		solic: { id: 0 }
	};

	componentDidMount() {
		this.loadSolics();
	}

	processMoney(event) {
		var api = new Api();
		this.setState({ _valor: api.ValorMoney(event.target.value) })
	}

	processNumber(vlr) {
		return new Api().ValorNum(vlr);
	}

	loadSolics = e => {
		this.setState({ loading: true, error: '', solic: { id: 0 } });
		var api = new Api();
		api.getTokenPortal('associadoSolicitacao', null).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {

				var strParcelas = 'Parcelamento: ';

				for (var i = 0; i < resp.payload.nuParcelas; i++) {
					var st = api.ValorMoney(resp.payload.lstParcelas[i]);
					if (st === '') st = '0,00';
					strParcelas += st;
					if (i < resp.payload.nuParcelas - 1)
						strParcelas += ' / ';
				}

				this.setState({
					loading: false,
					solic: resp.payload,
					strParcelas: strParcelas
				});
			}
		});
	}

	executeSolic = e => {
		e.preventDefault();
		var falhou = false;
		if (this.state._stSenha === undefined || this.state._stSenha === null)
			falhou = true;
		else if (this.state._stSenha.length !== 4)
			falhou = true;
		if (falhou === true) {
			this.setState({ error: 'Favor informar uma senha corretamente' });
			return;
		}
		var _solic = this.state.solic;
		_solic.stSenha = this.state._stSenha;
		this.setState({ loading: true, error: '', aviso: '', solic: _solic });
		var api = new Api();
		api.postTokenPortal("confirmaSolicitacao", JSON.stringify(_solic)).then(resp => {
			if (resp.ok === true) {
				this.setState({
					loading: false,
					aviso: "Venda confirmada com sucesso!",
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
				error: err.msg
			});
		});
	}

	render() {
		return (
			<div className={s.root}>
				<ol className="breadcrumb">
					<li className="breadcrumb-item">Portal </li>
					<li className="active breadcrumb-item">
						Solicitações em aberto
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
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
										<h5>Loja </h5>
										<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
											type="text"
											id="fieldName"
											maxLength="50"
											value={this.state.solic.stLoja}
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
															maxLength="10"
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
															maxLength="2"
															value={this.state.solic.nuParcelas}
															disabled
														/>
													</Col>
												</FormGroup>
											</td>
										</tr>
									</tbody>
								</table>
								<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
									type="text"
									id="fieldName"
									maxLength="2"
									value={this.state.strParcelas}
									disabled
								/>
								<br></br>
								<div align='center'>
									<table>
										<tbody>
											<tr>
												<td width='20px'></td>
												<td width='120px'>
													<h5>Senha Cartão</h5>
												</td>
												<td width='20px'></td>
												<td width='200px'>
													<InputGroup className="input-group-no-border px-4">
														<InputGroupAddon addonType="prepend">
															<InputGroupText>
																<i className="fa fa-lock text-white" />
															</InputGroupText>
														</InputGroupAddon>
														<Input id="password-input" type="password" className="input" width='80px' maxLength="4" value={this.state._stSenha} autoComplete='off'
															onChange={event => this.setState({ _stSenha: this.processNumber(event.target.value) })} />
													</InputGroup>
												</td>
											</tr>
										</tbody>
									</table>
									<br></br>
								</div>
							</Widget>
							<br></br>
							<div align='center'>
								<Button color="success"
									style={{ width: "200px" }}
									onClick={this.executeSolic}
									disabled={this.state.loading} >
									{this.state.loading === true ? (
										<span className="spinner">
											<i className="fa fa-spinner fa-spin" />
											&nbsp;&nbsp;&nbsp;
										</span>
									) : (
											<div />
										)}
									Confirmar Solicitação
								</Button>
							</div>
							<br></br>
						</div>
				}
			</div>
		)
	}
}
