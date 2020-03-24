
import React  from 'react';

import Widget from '../../../components/Widget';

import s from './Parcelamentos.module.scss';

import { Api } from '../../../shared/Api.js'

export default class AssociadoParcelamentos extends React.Component {

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

		api.getTokenPortal('associadoExtratoFuturo', null).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {
				this.setState({
					loading: false,
					parcelamento: resp.payload.parcelamento,					
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
						Parcelamentos em aberto
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				<Widget>
					{this.state.parcelamento !== undefined ? <div>
						<table>
							<thead>
								<tr>
								<th width='110px'>Mês / Ano</th>
								<th></th>
								<th width='90px'>Valor R$</th>
								<th></th>
								<th width='70px'>Pct.</th>
								<th></th>
								<th width='90px'>Disp. R$</th>
								</tr>
							</thead>
							<tbody>
								{this.state.parcelamento.map((current, index) => (
									<tr key={`${current}${index}`}>
										<td>
											{current.mesAno}
										</td>
										<td>&nbsp;</td>
										<td>
											{current.valor}
										</td>
										<td>&nbsp;</td>
										<td>
											{current.pctComprometido}%
										</td>
										<td>&nbsp;</td>
										<td>
											{current.vrDisponivel}
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
