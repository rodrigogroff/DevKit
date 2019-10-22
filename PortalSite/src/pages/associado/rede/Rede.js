
import React from 'react';

import Widget from '../../../components/Widget';

import s from './Rede.module.scss';

import { Api } from '../../../shared/Api.js'

export default class AssociadoRede extends React.Component {

	state = {
		loading: false,
	};

	componentDidMount() {

		this.setState({ loading: true, error: '' });

		var api = new Api();

		api.getTokenPortal('associadoRede', null).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else {
				this.setState({
					loading: false,
					lojas: resp.payload.resultados,					
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
						Rede credenciada
								{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				<Widget>
				{this.state.lojas !== undefined ? <div>
						<table width='100%'>
							<thead>
								<th>Loja / Endereço</th>
							</thead>
							<tbody>
								{this.state.lojas.map((current, index) => (
									<tr key={`${current}${index}`}>
										<td>
											<b>{current.nomeLoja}</b> <br></br>
											{current.end}<br></br>
											<br></br>
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
			</div >
		)
	}
}
