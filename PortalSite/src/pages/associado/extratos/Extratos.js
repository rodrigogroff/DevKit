
import React from 'react';

import {
	Col,
	FormGroup,
	Label,
	Input,
} from 'reactstrap';
import Widget from '../../../components/Widget';

import s from './Extratos.module.scss';

import { Api } from '../../../shared/Api.js'

export default class AssociadoExtratos extends React.Component {

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

		api.getTokenPortal('associadoExtratoAtual', null).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {
				this.setState({
					loading: false,
					mesAtual: resp.payload.mesAtual,
					totalExtrato: resp.payload.totalExtrato,
					saldoDisponivel: resp.payload.saldoDisponivel,
					vendas: resp.payload.vendas
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
						Extrato de fatura atual
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				<Widget>

					<h3>{this.state.mesAtual}</h3>

					<table width='100%'>
						<tbody>
							<tr>
								<td>
									<FormGroup row>
										<Label for="normal-field" md={4} className="text-md-left">
											<h4>Total Extrato</h4></Label>
										<Col md={8}>
											<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
												type="text"
												id="fieldName"
												autocomplete='off'
												maxLength="50"
												value={this.state.totalExtrato}
												disabled
											/>
										</Col>
									</FormGroup>
								</td>
								<td width='20px'></td>
								<td>
									<FormGroup row>
										<Label for="normal-field" md={4} className="text-md-left">
											<h4>Saldo</h4></Label>
										<Col md={8}>
											<Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
												type="text"
												id="fieldName"
												maxLength="50"
												value={this.state.saldoDisponivel}
												disabled
											/>
										</Col>
									</FormGroup>
								</td>
							</tr>
						</tbody>
					</table>
					{this.state.vendas !== undefined ? <div>
						<table width='100%'>
							<thead>
								<tr>
									<th>Data</th>
									<th></th>
									<th>Valor R$</th>
									<th></th>
									<th>Parcela</th>
									<th></th>
									<th>Loja</th>
								</tr>
							</thead>
							<tbody>
								{this.state.vendas.map((current, index) => (
									<tr key={`${current}${index}`}>
										<td>
											{current.dtVenda}
										</td>
										<td>&nbsp;</td>
										<td>
											{current.valor}
										</td>
										<td>&nbsp;</td>
										<td>
											{current.parcela}
										</td>
										<td>&nbsp;</td>
										<td>
											{current.estab}
										</td>
									</tr>
								))}
							</tbody>
						</table>
					</div>
						:
						<div></div>
					}
				</Widget>
			</div>
		)
	}
}
