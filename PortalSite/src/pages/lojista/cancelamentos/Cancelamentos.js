
import React from 'react';

import {
	Modal,
	ModalHeader,
	ModalBody,
	ModalFooter,
	Button,
} from 'reactstrap';

import s from './Cancelamentos.module.scss';

import { Api } from '../../../shared/Api.js'

export default class LojistaCancelamentos extends React.Component {

	state = {
		loading: false,
		currentCanc: undefined,
		error: '',
		msg: '',
		results: [],
	};

	componentDidMount() {
		this.loadSolics();
	}

	loadSolics = e => {

		this.setState({ loading: true, error: '', results: [] });

		var api = new Api();

		api.getTokenPortal('lojistaCancelamentos', null).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {
				this.setState({
					loading: false,
					results: resp.payload.results,
				});
			}
		});
	}

	cancelarTransacao = e => {
		
		this.setState({ loading: true });

		var api = new Api();
		var id = this.state.currentCanc.id;
		var serviceData = JSON.stringify({ id });

		api.postTokenPortal('solicitaVendaCancelamentoPOS', serviceData).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {
				
				this.loadSolics();

				this.setState({
					currentCanc: undefined,
					loading: false,					
					msg: 'Venda cancelada com sucesso!'
				});				
			}
		});
	}

	render() {
		return (
			<div className={s.root}>
				<ol className="breadcrumb">
					<li className="breadcrumb-item">Portal </li>
					<li className="active breadcrumb-item">
						Autorizações para cancelamentos
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>

				<Modal isOpen={this.state.msg.length > 0} toggle={() => this.setState({ msg: "" })}>
					<ModalHeader toggle={() => this.setState({ msg: "" })}>
						Aviso do Sistema
            		</ModalHeader>
					<ModalBody className="bg-success-system">
						<div className="modalBodyMain">
							<br />
							{this.state.msg}
							<br />
							<br />
						</div>
					</ModalBody>
					<ModalFooter className="bg-white">
						<Button color="primary" onClick={() => this.setState({ msg: "" })}> Fechar </Button>
					</ModalFooter>
				</Modal>

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

				<Modal isOpen={this.state.currentCanc !== undefined}>
					<ModalHeader toggle={() => this.setState({ currentCanc: undefined })}>
						Cancelamento de transação
            					</ModalHeader>
					<ModalBody className="bg-password-system">
						<div className="modalBodyMain">
							<br />
							<div align='center'>
								{
									this.state.currentCanc !== undefined ?
										<div>
											<table>
												<tbody>
													<tr>
														<td width='50px' align='left'>&nbsp;</td>
														<td width='90px' align='left'>Data</td>
														<td width='180px' align='left'>{this.state.currentCanc.dt}</td>
													</tr>
													<tr>
														<td></td>
														<td align='left'>Valor</td>
														<td align='left'>{this.state.currentCanc.valor}</td>
													</tr>
													<tr>
														<td></td>
														<td align='left'>Cartão</td>
														<td align='left'>{this.state.currentCanc.cartao}</td>
													</tr>
													<tr>
														<td>&nbsp;</td>
													</tr>
													<tr>
														<td></td>
														<td colSpan="2">	

														<i><p>A operação acima não poderá ser desfeita. Aperte o botão abaixo para continuar com o cancelamento da transação.</p></i>
														<br></br>

														<Button color="danger"
															style={{ width: "200px" }}
															onClick={this.cancelarTransacao}
															disabled={this.state.loading} >
															{this.state.loading === true ? (
																<span className="spinner">
																	<i className="fa fa-spinner fa-spin" />
																	&nbsp;&nbsp;&nbsp;
																</span>
															) : (
																	<div />
																)}
															Confirmar
														</Button>
														</td>
													</tr>
												</tbody>
											</table>
											<br></br>

										</div> : <div></div>
								}
							</div>
							<br />
						</div>
					</ModalBody>
					<ModalFooter className="bg-white">
						<Button color="primary" onClick={() => this.setState({ currentCanc: undefined })}> Fechar </Button>
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
					this.state.results.length > 0 ?
						<div>
							<table width='100%'>
								<thead>
									<tr>
										<th>Data</th>
										<th></th>
										<th>Valor R$</th>
										<th></th>
										<th>Cartão</th>
										<th></th>
										<th>Ação</th>
									</tr>
								</thead>
								<tbody>
									{this.state.results.map((current, index) => (
										<tr key={`${current}${index}`}>
											<td>
												{current.dt}
											</td>
											<td>&nbsp;</td>
											<td>
												{current.valor}
											</td>
											<td>&nbsp;</td>
											<td>
												{current.cartao}
											</td>
											<td>&nbsp;</td>
											<td>
												<Button color="danger"
													style={{ width: "90px", height: "23px" }}
													onClick={() => this.setState({ currentCanc: current })}
													disabled={this.state.loading} >
													<p style={{ marginTop: "-6px", fontSize: "11px" }}>Cancelar</p>
												</Button>
											</td>
										</tr>
									))}
								</tbody>
							</table>
						</div> :
						<div align='center'>
							<br></br>
							<h3>Nenhum registro encontrado</h3>
						</div>
				}
			</div>
		)
	}
}
