
import React from 'react';

import Widget from '../../../components/Widget';
import {
	Button,
	Input,
} from "reactstrap";
import s from './Rede.module.scss';

import { Api } from '../../../shared/Api.js'

export default class AssociadoRede extends React.Component {

	state = {
		loading: false,
		_pesquisa: '',
	};

	componentDidMount() {
		this.setState({ loading: true, error: '' });
		this.pesquisar();
	}

	pesquisar = e => {
		var api = new Api();
		api.getTokenPortal('associadoRede', 'pesquisa=' + this.state._pesquisa).then(resp => {
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

					<table>
						<tbody>
							<tr>
								<td width='90px'>
									Pesquisar
								</td>
								<td width='90px'>
									<Input className="input-transparent form-control" id="pesquisa-input" autocomplete='off'
										onChange={event => this.setState({ _pesquisa: event.target.value })} />
								</td>
								<td width='20px'></td>
								<td width='120px'>
									<Button className={s.block} color="primary" onClick={this.pesquisar}>Pesquisar</Button>
								</td>
							</tr>
						</tbody>
					</table>
					<br></br>

					{this.state.lojas !== undefined ? <div>
						<table width='100%'>
							<thead>
								<tr>
									<th>Loja / Endereço</th>
								</tr>								
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
