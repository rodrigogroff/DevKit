
import React from 'react';
import QrReader from 'react-qr-reader'

import {
	Button,
	Modal,
	ModalHeader,
	ModalBody,
	ModalFooter,
	Input,
	InputGroup,
} from "reactstrap";

import s from './venda.module.scss';

import { Api } from '../../../shared/Api.js'

export default class LojistaVenda extends React.Component {

	constructor(props) {
		super(props);

		this.state = {
			width: 0,
			height: 0,
			error: "",
			loading: false,
			digitar: true,
			naoEscolheu: false,
			result: 'Pendente de leitura...',
			_valor: "0,00",
			_parcelas: "1"
		};
	}

	handleScan = data => {
		if (data) {
			this.setState({
				result: data
			})
		}
	}
	handleError = err => {
		console.error(err)
	}

	setaDigitado = e => {
		this.setState({
			naoEscolheu: false,
			digitar: true
		})
	}

	setaScan = e => {
		this.setState({
			naoEscolheu: false,
			digitar: false
		})
	}

	executeSolic = e => {
		e.preventDefault();

		var empresa = this.state._empresa;
		var matricula = this.state._matricula;
		var codAcesso = this.state._codAcesso;
		var venc = this.state._venc;
		var valor = this.state._valor;
		var parcelas = this.state._parcelas;

		var serviceData = JSON.stringify({ empresa, matricula, codAcesso, venc, valor, parcelas });

		var api = new Api();

		api.postTokenPortal("solicitaVenda", serviceData).then(resp => {
			if (resp.ok === true) {
				//resp.payload;
			}
			else {
				this.setState({
					loading: false,
					alertIsOpen: true,
					error: resp.msg
				});
			}
		}).catch(err => {
			this.setState({
				loading: false,
				alertIsOpen: true,
				error: "Nao foi possivel verificar os dados de sua requisição"
			});
		});
	}

	render() {
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

				<ol className="breadcrumb">
					<li className="breadcrumb-item">Portal Lojista </li>
					<li className="active breadcrumb-item">
						Venda
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>

				{
					this.state.naoEscolheu === true ?
						<div>
							<table width='100%'>
								<tbody>
									<tr>
										<td width='50%'>											
											<Button className={s.block} color="primary" onClick={this.setaDigitado}>Digitado</Button>
										</td>
										<td width='20px'></td>
										<td width='50%'>
											<Button className={s.block} color="warning" onClick={this.setaScan}>Cartão Virtual</Button>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
						:
						<div>
							{
								this.state.digitar === false ?
									<div>										
										<div align='center' style={{ display: 'none' }}>
											<br></br>
											<QrReader
												delay={300}
												onError={this.handleError}
												onScan={this.handleScan}
												style={{ width: '100%' }}
											/>
											<p>{this.state.result}</p>
										</div>
									</div>
									:
									<div>
										
									</div>
							}

							<div align='center'>

								<h4>Digitado</h4>
								<p> Informe o número do Cartão Benefícios ConveyNET</p>

								<InputGroup >
									<table >
										<tbody>
											<tr>
												<td>
													<Input className="input-transparent form-control" id="empresa-input" maxLength="6" type="tel" pattern="[0-9]*" inputmode="numeric"
														onChange={event => this.setState({ _empresa: event.target.value })} />
												</td>
												<td width='10px'></td>
												<td >
													<Input className="input-transparent form-control" id="matricula-input" maxLength="6" type="tel" pattern="[0-9]*" inputmode="numeric"
														onChange={event => this.setState({ _matricula: event.target.value })} />
												</td>
												<td width='10px'></td>
												<td >
													<Input className="input-transparent form-control" id="codAcesso-input" maxLength="4" type="tel" pattern="[0-9]*" inputmode="numeric"
														onChange={event => this.setState({ _codAcesso: event.target.value })} />
												</td>
												<td width='10px'></td>
												<td >
													<Input className="input-transparent form-control" id="vencimento-input" maxLength="4" type="tel" pattern="[0-9]*" inputmode="numeric"
														onChange={event => this.setState({ _venc: event.target.value })} />
												</td>
											</tr>
										</tbody>
									</table>
								</InputGroup>
								<br></br>
								<InputGroup >
									<table >
										<tbody>
											<tr height='42px'>
												<td width='90px'>Valor</td>
												<td width='150px'>
													<Input className="input-transparent form-control" id="valor" maxLength="9" type="tel" pattern="[0-9]*" inputmode="numeric"
														value={this.state._valor} onChange={event => this.setState({ _valor: event.target.value })} />
												</td>
												<td width='60px'></td>
												<td width='90px'>Parcelas</td>
												<td width='50px'>
													<Input className="input-transparent form-control" id="parcelas" maxLength="6" type="tel" pattern="[0-9]*" inputmode="numeric"
														value={this.state._parcelas} onChange={event => this.setState({ _parcelas: event.target.value })} />
												</td>
											</tr>
										</tbody>
									</table>
								</InputGroup>
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
									Solicitar Venda
								</Button>
							</div>
						</div>

				}

				<br></br>
				<br></br>
			</div >
		)
	}
}
