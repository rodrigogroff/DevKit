
import React from 'react';

import {
	Col,
	FormGroup,
	Label,
	Input,
} from 'reactstrap';
import Widget from '../../../components/Widget';

import { CircularProgressbar, buildStyles } from "react-circular-progressbar";
import "react-circular-progressbar/dist/styles.css";
import s from './Limites.module.scss';

import { Api } from '../../../shared/Api.js'

export default class AssociadoLimites extends React.Component {

	state = {
		loading: false,
		limiteCartao: '',
		parcelas: '',
		limiteMensalDisp: '',
		cotaExtra: '',
		melhorDia: '',
		mensalUtilizado: '',
		mesVigente: '',
		pct: '',
	};

	componentDidMount() {

		this.setState({ loading: true, error: '' });

		var api = new Api();

		api.getTokenPortal('associadoLimites', null).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {
				this.setState({
					loading: false,
					limiteCartao: resp.payload.limiteCartao,
					parcelas: resp.payload.parcelas,
					limiteMensalDisp: resp.payload.limiteMensalDisp,
					cotaExtra: resp.payload.cotaExtra,
					melhorDia: resp.payload.melhorDia,
					mensalUtilizado: resp.payload.mensalUtilizado,
					mesVigente: resp.payload.mesVigente,
					pct: resp.payload.pct,
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
						Limites do Associado
								{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				<Widget>
					<FormGroup row>
						<Label for="normal-field" md={4} className="text-md-left">
							<h4>Vigência</h4></Label>
						<Col md={7}>
							<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
								type="text"
								id="fieldName"
								maxLength="50"
								value={this.state.mesVigente}
								disabled
								 />
						</Col>
					</FormGroup>
					<table>
						<tbody>
							<tr>
								<td>
									<FormGroup row>
										<Label for="normal-field" md={4} className="text-md-left">
											<h4>Limite Mensal</h4>
										</Label>
										<Col md={7}>
											<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
												type="text"
												id="fieldName"
												maxLength="50"
												value={this.state.limiteCartao}
												disabled
												 />
										</Col>
									</FormGroup>
								</td>
								<td width='20px'></td>
								<td>
									<FormGroup row>
										<Label for="normal-field" md={4} className="text-md-left">
											<table>
												<tbody>
													<tr>
														<td>
															<h4>Disponível</h4>
														</td>
														<td width='20px'>
														</td>
														<td valign='top'>
															<div style={{ width: '20px' }}>
																<CircularProgressbar
																	value={this.state.pct} text=''
																	background counterClockwise backgroundPadding={6}
																	styles={buildStyles({
																		backgroundColor: "#3a3a3a",
																		textColor: "#ffffff",
																		pathColor: "#fff",
																		trailColor: "#708294"
																	})} />
															</div>
														</td>
													</tr>
												</tbody>
											</table>
										</Label>
										<Col md={7}>
											<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
												type="text"
												id="fieldName"
												maxLength="50"
												value={this.state.limiteMensalDisp}
												disabled
												 />
										</Col>
									</FormGroup>
								</td>
							</tr>
						</tbody>
					</table>

					<FormGroup row>
						<Label for="normal-field" md={4} className="text-md-left">
							<h4>Parcela mensal utilizada </h4>
						</Label>
						<Col md={7}>
							<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
								type="text"
								id="fieldName"
								maxLength="50"
								value={this.state.mensalUtilizado}
								disabled
								 />
						</Col>
					</FormGroup>

					<FormGroup row>
						<Label for="normal-field" md={4} className="text-md-left">
							<h4>Cota Extra </h4>
						</Label>
						<Col md={7}>
							<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
								type="text"
								id="fieldName"
								maxLength="50"
								value={this.state.cotaExtra}
								disabled
								 />
						</Col>
					</FormGroup>
					<table>
						<tbody>
							<tr>
								<td>
									<FormGroup row>
										<Label for="normal-field" md={4} className="text-md-left">
											<h4>Parcelamento</h4></Label>
										<Col md={7}>
											<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
												type="text"
												id="fieldName"
												maxLength="50"
												value={this.state.parcelas}
												disabled
												/>
										</Col>
									</FormGroup>
								</td>
								<td width='20px'></td>
								<td>
									<FormGroup row>
										<Label for="normal-field" md={4} className="text-md-left">
											<h4>Melhor Dia</h4></Label>
										<Col md={7}>
											<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
												type="text"
												id="fieldName"
												maxLength="50"
												value={this.state.melhorDia}
												disabled
												 />
										</Col>
									</FormGroup>
								</td>
							</tr>
						</tbody>
					</table>
				</Widget >
			</div >
		)
	}
}
