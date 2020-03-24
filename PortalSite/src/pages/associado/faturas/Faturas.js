
import React from 'react';

import Widget from '../../../components/Widget';

import "react-circular-progressbar/dist/styles.css";
import s from './Faturas.module.scss';

import { Api } from '../../../shared/Api.js'

export default class AssociadoFaturas extends React.Component {

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

		api.getTokenPortal('associadoExtratoFechada', null).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {
				this.setState({
					loading: false,
					faturas: resp.payload.faturas,
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
						Faturas fechadas
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				{this.state.faturas !== undefined ? <div>
					{this.state.faturas.map((current, index) => (
						<Widget key={`${current}${index}`}>
							<h4>{current.mesAno}</h4>
							<p>Valor: {current.valorTotal}</p>
							<table width='100%'>
								<thead>
									<tr>									
									<th width='110px'>Data</th>
									<th></th>
									<th width='90px'>Valor R$</th>
									<th></th>
									<th width='70px'>Parcela</th>
									<th></th>
									<th >Estab.</th>
									</tr>
								</thead>
								<tbody>
									{current.vendas.map((currentVenda, index) => (
										<tr key={`${currentVenda}${index}`}>
											<td>
												{currentVenda.dtVenda}
											</td>
											<td>&nbsp;</td>
											<td>
												{currentVenda.valor}
											</td>
											<td>&nbsp;</td>
											<td>
												{currentVenda.parcela}
											</td>
											<td>&nbsp;</td>
											<td>
												{currentVenda.estab}
											</td>
										</tr>
									))}
								</tbody>
							</table>
						</Widget>
					))}
					<br></br>
					<p>Para visualizar períodos superiores a 6 meses, solicite a  entidade mantenedora do seu cartão</p>
					<br></br>
					<br></br>
					<br></br>
				</div>
					:
					<div></div>}
			</div>
		)
	}
}
