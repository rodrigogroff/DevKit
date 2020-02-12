
import React from 'react';
import { Redirect } from "react-router-dom";

import QrReader from 'react-qr-reader'

import {
	Button,
	FormGroup,
	Label,
	Modal,
	ModalHeader,
	ModalBody,
	ModalFooter,
	Input,
	InputGroup,
	InputGroupAddon,
	InputGroupText,
} from "reactstrap";

import Widget from "../../../components/Widget";
import s from './venda.module.scss';

import { Api } from '../../../shared/Api.js'

export default class LojistaVenda extends React.Component {

	constructor(props) {
		super(props);

		this.state = {
			width: 0,
			height: 0,
			error: "",
			aviso: "",
			loading: false,
			digitar: true,
			vendaPos: true,
			vendaMobile: false,
			vendaMobile_digitado: true,
			vendaMobile_qrcode: false,
			redirVenda: false,
			tipoVenda: 'pos',
			tipoVendaMobile: 'digitado',
			result: 'Pendente de leitura...',
			_valor: "0,00",
			_parcelas: "1"
		};

		this.processMoney = this.processMoney.bind(this);
		this.processMoneyP1 = this.processMoneyP1.bind(this);
		this.processMoneyP2 = this.processMoneyP2.bind(this);
		this.processMoneyP3 = this.processMoneyP3.bind(this);
		this.processMoneyP4 = this.processMoneyP4.bind(this);
		this.processMoneyP5 = this.processMoneyP5.bind(this);
		this.processMoneyP6 = this.processMoneyP6.bind(this);
		this.processMoneyP7 = this.processMoneyP7.bind(this);
		this.processMoneyP8 = this.processMoneyP8.bind(this);
		this.processMoneyP9 = this.processMoneyP9.bind(this);
		this.processMoneyP10 = this.processMoneyP10.bind(this);
		this.processMoneyP11 = this.processMoneyP11.bind(this);
		this.processMoneyP12 = this.processMoneyP12.bind(this);
	}

	handleScan = data => {
		if (data) {
			this.setState({ result: data }, () => {
				var field = 0;
				var __empresa = '';
				var __matricula = '';
				var __codAcesso = '';
				var __venc = '';

				for (let i = 0; i < data.length; ++i) {
					if (data[i] === '.') field = field + 1;
					else
						switch (field) {
							case 0: __empresa += data[i]; break;
							case 1: __matricula += data[i]; break;
							case 2: __codAcesso += data[i]; break;
							case 3: __venc += data[i]; break;
						}
				}

				this.setState({ _empresa: __empresa, _matricula: __matricula, _codAcesso: __codAcesso, _venc: __venc });
			});
		}
	}

	forceQR = () => {
		this.handleScan('2.1.0534.0716');
	}

	handleError = err => {
		console.error(err)
	}

	processMoney(event) {
		var api = new Api();
		this.setState({ _valor: api.ValorMoney(event.target.value) })
	}

	processParcela(event) {
		var api = new Api();
		this.setState({ _parcelas: api.ValorNum(event.target.value) })
	}

	processMoneyP1(event) { var api = new Api(); this.setState({ _valorP1: api.ValorMoney(event.target.value) }) }
	processMoneyP2(event) { var api = new Api(); this.setState({ _valorP2: api.ValorMoney(event.target.value) }) }
	processMoneyP3(event) { var api = new Api(); this.setState({ _valorP3: api.ValorMoney(event.target.value) }) }
	processMoneyP4(event) { var api = new Api(); this.setState({ _valorP4: api.ValorMoney(event.target.value) }) }
	processMoneyP5(event) { var api = new Api(); this.setState({ _valorP5: api.ValorMoney(event.target.value) }) }
	processMoneyP6(event) { var api = new Api(); this.setState({ _valorP6: api.ValorMoney(event.target.value) }) }
	processMoneyP7(event) { var api = new Api(); this.setState({ _valorP7: api.ValorMoney(event.target.value) }) }
	processMoneyP8(event) { var api = new Api(); this.setState({ _valorP8: api.ValorMoney(event.target.value) }) }
	processMoneyP9(event) { var api = new Api(); this.setState({ _valorP9: api.ValorMoney(event.target.value) }) }
	processMoneyP10(event) { var api = new Api(); this.setState({ _valorP10: api.ValorMoney(event.target.value) }) }
	processMoneyP11(event) { var api = new Api(); this.setState({ _valorP11: api.ValorMoney(event.target.value) }) }
	processMoneyP12(event) { var api = new Api(); this.setState({ _valorP12: api.ValorMoney(event.target.value) }) }

	processNumber(vlr) {
		return new Api().ValorNum(vlr);
	}

	trocarTipoVenda = e => {
		if (this.state.vendaPos) {
			this.setState({
				tipoVenda: 'mobile',
				vendaPos: false,
				vendaMobile: true,
				tipoVendaMobile: 'digitado',
				vendaMobile_digitado: true,
				vendaMobile_qrcode: false,
			})
		}
		else {
			this.setState({
				tipoVenda: 'pos',
				vendaPos: true,
				vendaMobile: false,
			})
		}
	}

	trocarTipoVendaMobile = e => {
		if (this.state.vendaMobile_digitado) {
			this.setState({
				tipoVendaMobile: 'qrcode',
				result: '',
				vendaMobile_digitado: false,
				vendaMobile_qrcode: true,
			})
		}
		else {
			this.setState({
				tipoVendaMobile: 'digitado',
				vendaMobile_digitado: true,
				vendaMobile_qrcode: false,
			})
		}
	}

	executeSolic = e => {

		e.preventDefault();

		var empresa = this.state._empresa;
		var matricula = this.state._matricula;
		var codAcesso = this.state._codAcesso;
		var venc = this.state._venc;
		var valor = this.state._valor;
		var parcelas = this.state._parcelas;

		if (empresa === "" || empresa === undefined) {
			this.setState({ loading: false, error: 'Empresa inválida!' });
			return;
		}

		if (matricula === "" || matricula === undefined) {
			this.setState({ loading: false, error: 'Matricula inválida!' });
			return;
		}

		if (codAcesso === "" || codAcesso === undefined) {
			this.setState({ loading: false, error: 'Codigo de Acesso inválida!' });
			return;
		}

		if (venc === "" || venc === undefined) {
			this.setState({ loading: false, error: 'Vencimento inválida!' });
			return;
		}

		if (this.state.tipoVenda !== 'mobile') {
			if (this.state._stSenha === "" || this.state._stSenha === undefined) {
				this.setState({ loading: false, error: 'Senha inválida!' });
				return;
			}
		}

		if (valor === "0,00" || valor === "") {
			this.setState({
				loading: false,
				error: 'Valor Inválido!'
			});
			return;
		}

		{
			var pi = parseInt(parcelas);
			if (pi <= 0 || pi >= 12) {
				this.setState({
					loading: false,
					error: 'Número de Parcelas inválido!'
				});
				return;
			}
		}

		this.setState({ loading: true });

		if (this.state.tipoVenda === 'mobile') {
			var serviceData = JSON.stringify({ empresa, matricula, codAcesso, venc, valor, parcelas });
			var api = new Api();
			api.postTokenPortal("solicitaVenda", serviceData).then(resp => {
				if (resp.ok === true) {
					this.setState({
						loading: false,
						aviso: "Solicitação de venda feita com sucesso!",
					});
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
		else {
			var senha = this.state._stSenha;
			var serviceData = JSON.stringify({ empresa, matricula, codAcesso, venc, valor, parcelas, senha });
			var api = new Api();
			api.postTokenPortal("solicitaVendaPos", serviceData).then(resp => {
				if (resp.ok === true) {
					this.setState({
						loading: false,
						aviso: "Venda POS feita com sucesso!",
					});
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
	}

	render() {
		if (this.state.redirVenda === true) return <Redirect to="/app/lojista/solics" />;
		else
			return (
				<div className={s.root}>
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
							<Button color="primary" onClick={() => this.setState({ aviso: "", redirVenda: true })}> Fechar </Button>
						</ModalFooter>
					</Modal>

					<ol className="breadcrumb">
						<li className="breadcrumb-item">Portal Lojista </li>
						<li className="active breadcrumb-item">
							Venda
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
						</li>
					</ol>

					<div align='center'>
						<Widget>
							<table width='100%'>
								<tbody>
									<tr>
										<td width='90px'><h5> Autorização</h5></td>
										<td width='20px'></td>
										<td width='90px' style={{ paddingTop: '8px' }}>
											<FormGroup className="radio abc-radio">
												<Input type="radio" id="radio1" name="r" onClick={this.trocarTipoVenda} checked={this.state.vendaPos} />
												<Label for="radio1" style={{ paddingTop: '4px' }}>POS</Label>
											</FormGroup>
										</td>
										<td width='90px' style={{ paddingTop: '8px' }}>
											<FormGroup className="radio abc-radio">
												<Input type="radio" id="radio2" name="r" onClick={this.trocarTipoVenda} checked={this.state.vendaMobile} />
												<Label for="radio2" style={{ paddingTop: '4px' }}>Mobile</Label>
											</FormGroup>
										</td>
									</tr>
									{this.state.tipoVenda === 'mobile' ?
										<tr>
											<td><h5>Captura</h5></td>
											<td ></td>
											<td style={{ paddingTop: '8px' }}>
												<FormGroup className="radio abc-radio">
													<Input type="radio" id="radio3" name="m" onClick={this.trocarTipoVendaMobile} checked={this.state.vendaMobile_digitado} />
													<Label for="radio3" style={{ paddingTop: '4px' }}>Digitado</Label>
												</FormGroup>
											</td>
											<td style={{ paddingTop: '8px' }}>
												<FormGroup className="radio abc-radio">
													<Input type="radio" id="radio4" name="m" onClick={this.trocarTipoVendaMobile} checked={this.state.vendaMobile_qrcode} />
													<Label for="radio4" style={{ paddingTop: '4px' }}>QRCode</Label>
												</FormGroup>
											</td>
										</tr>
										:
										<div></div>
									}
								</tbody>
							</table>
						</Widget>

						<p> Informe o número do Cartão Benefícios ConveyNET</p>
						<InputGroup >
							<table>
								<tbody>
									<tr>
										<td>
											<Input className="input-transparent form-control" id="empresa-input" maxLength="6" type="tel" pattern="[0-9]*" inputmode="numeric" value={this.state._empresa}
												onChange={event => this.setState({ _empresa: this.processNumber(event.target.value) })} />
										</td>
										<td width='10px'></td>
										<td >
											<Input className="input-transparent form-control" id="matricula-input" maxLength="6" type="tel" pattern="[0-9]*" inputmode="numeric" value={this.state._matricula}
												onChange={event => this.setState({ _matricula: this.processNumber(event.target.value) })} />
										</td>
										<td width='10px'></td>
										<td >
											<Input className="input-transparent form-control" id="codAcesso-input" maxLength="4" type="tel" pattern="[0-9]*" inputmode="numeric" value={this.state._codAcesso}
												onChange={event => this.setState({ _codAcesso: this.processNumber(event.target.value) })} />
										</td>
										<td width='10px'></td>
										<td >
											<Input className="input-transparent form-control" id="vencimento-input" maxLength="4" type="tel" pattern="[0-9]*" inputmode="numeric" value={this.state._venc}
												onChange={event => this.setState({ _venc: this.processNumber(event.target.value) })} />
										</td>
									</tr>
								</tbody>
							</table>
						</InputGroup>
						<br></br>
						<InputGroup >
							<table width='100%'>
								<tbody>
									<tr height='42px'>
										<td width='90px'>Valor</td>
										<td width='150px'>
											<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
												value={this.state._valor} onChange={this.processMoney} />
										</td>
										<td width='60px'></td>
										<td width='90px'>Parcelas</td>
										<td width='50px'>
											<Input className="input-transparent form-control" id="parcelas" maxLength="2" type="tel" pattern="[0-9]*" inputmode="numeric"
												onChange={event => this.setState({ _parcelas: this.processNumber(event.target.value) })} />
										</td>
									</tr>
								</tbody>
							</table>
						</InputGroup>
						<br></br>
						{
							this.state.tipoVenda === 'mobile' ?
								<div>
									{
										this.state.vendaMobile_digitado === true || (this.state.vendaMobile_qrcode === true && this.state.result) ?
											<div>
												<Button color="primary"
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
													Solicitar Autorização
													</Button>
											</div>
											:
											<div>
												{
													this.state.result !== '' ? <div></div>
														:
														<div>
															<h5>Leia o QRCODE no celular do associado</h5>
															<br></br>
															<QrReader
																delay={300}
																onError={this.handleError}
																onScan={this.handleScan}
																style={{ width: '80%' }}
															/>
															<br></br>
															<p>{this.state.result}</p>
															<br></br>
															<Button color="primary"
																style={{ width: "200px" }}
																onClick={this.forceQR}
																disabled={this.state.loading} >
																{this.state.loading === true ? (
																	<span className="spinner">
																		<i className="fa fa-spinner fa-spin" />
																		&nbsp;&nbsp;&nbsp;
															</span>
																) : (
																		<div />
																	)}
																Testar
															</Button>
														</div>
												}

											</div>
									}
								</div>
								:
								<div>
									{this.state._parcelas >= 2 && this.state._parcelas > 0 ?
										<div>
											<table>
												<tbody>
													<tr>
														<td width='90px' >Parc. 1</td>
														<td width='90px'>
															{
																this.state._parcelas >= 2 && this.state._parcelas > 0 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP1} onChange={this.processMoneyP1} />
																	</div>
																	: <div></div>
															}
														</td>
														<td width='10px'> </td>
														<td width='90px'>Parc. 2</td>
														<td width='90px'>
															{
																this.state._parcelas >= 2 && this.state._parcelas > 0 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP2} onChange={this.processMoneyP2} />
																	</div>
																	: <div></div>
															}
														</td>
													</tr>
													<tr>
														<td>
															{
																this.state._parcelas >= 3 ?
																	<div>Parc. 3</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 3 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP3} onChange={this.processMoneyP3} />
																	</div> : <div></div>
															}
														</td>
														<td></td>
														<td>
															{
																this.state._parcelas >= 4 ?
																	<div>Parc. 4</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 4 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP4} onChange={this.processMoneyP4} />
																	</div> : <div></div>
															}
														</td>
													</tr>


													<tr>
														<td>
															{
																this.state._parcelas >= 5 ?
																	<div>Parc. 5</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 5 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP5} onChange={this.processMoneyP5} />
																	</div> : <div></div>
															}
														</td>
														<td></td>
														<td>
															{
																this.state._parcelas >= 6 ?
																	<div>Parc. 6</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 6 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP6} onChange={this.processMoneyP6} />
																	</div> : <div></div>
															}
														</td>
													</tr>

													<tr>
														<td>
															{
																this.state._parcelas >= 7 ?
																	<div>Parc. 7</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 7 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP7} onChange={this.processMoneyP7} />
																	</div> : <div></div>
															}
														</td>
														<td></td>
														<td>
															{
																this.state._parcelas >= 8 ?
																	<div>Parc. 8</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 8 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP8} onChange={this.processMoneyP8} />
																	</div> : <div></div>
															}
														</td>
													</tr>

													<tr>
														<td>
															{
																this.state._parcelas >= 9 ?
																	<div>Parc. 9</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 9 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP9} onChange={this.processMoneyP9} />
																	</div> : <div></div>
															}
														</td>
														<td></td>
														<td>
															{
																this.state._parcelas >= 10 ?
																	<div>Parc. 10</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 10 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP10} onChange={this.processMoneyP10} />
																	</div> : <div></div>
															}
														</td>
													</tr>

													<tr>
														<td>
															{
																this.state._parcelas >= 11 ?
																	<div>Parc. 11</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 11 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP11} onChange={this.processMoneyP11} />
																	</div> : <div></div>
															}
														</td>
														<td></td>
														<td>
															{
																this.state._parcelas >= 12 ?
																	<div>Parc. 12</div> : <div></div>
															}
														</td>
														<td>
															{
																this.state._parcelas >= 12 ?
																	<div>
																		<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
																			value={this.state._valorP12} onChange={this.processMoneyP12} />
																	</div> : <div></div>
															}
														</td>
													</tr>

												</tbody>
											</table>

										</div> : <div></div>
									}

									<br></br>

									<Widget>
										<FormGroup row>
											<Label for="normal-field" md={12} className="text-md-left">
												<h5>Senha Cartão</h5>
											</Label>
											<InputGroup className="input-group-no-border px-4">
												<InputGroupAddon addonType="prepend">
													<InputGroupText>
														<i className="fa fa-lock text-white" />
													</InputGroupText>
												</InputGroupAddon>
												<Input id="password-input" type="password" className="input" width='80px' maxLength="4" value={this.state._stSenha}
													onChange={event => this.setState({ _stSenha: this.processNumber(event.target.value) })} />
											</InputGroup>
										</FormGroup>
									</Widget>
									<br></br>
									<Button color="primary"
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
										Efetuar Venda
									</Button>
								</div>
						}
						<br></br>
						<br></br>
					</div >
				</div >
			)
	}
}
