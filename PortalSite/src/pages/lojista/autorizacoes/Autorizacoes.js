
import React from 'react';

import {
	Modal,
	ModalHeader,
	ModalBody,
	ModalFooter,
	Button,
} from 'reactstrap';

import s from './Autorizacoes.module.scss';

import { Api } from '../../../shared/Api.js'

export default class LojistaAutorizacoes extends React.Component {

	state = {
		loading: false,
		error: '',
		results: [],
	};

	componentDidMount() {
		this.loadSolics();
	}

	loadSolics = e => {

		this.setState({ loading: true, error: '' });

		var api = new Api();

		api.getTokenPortal('lojistaAutorizacoes', null).then(resp => {
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

	render() {
		return (
			<div className={s.root}>
				<ol className="breadcrumb">
					<li className="breadcrumb-item">Portal </li>
					<li className="active breadcrumb-item">
						Histórico de Autorizações
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>

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

				<table width='100%'>
					<thead>
						<tr>
							<th>Data</th>
							<th></th>
							<th>Valor R$</th>
							<th></th>
							<th>Cartão</th>
							<th></th>
							<th>Situação</th>
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
									{current.sit}
								</td>
							</tr>
						))}
					</tbody>
				</table>

			</div>
		)
	}
}
