
import React from 'react';
import QrReader from 'react-qr-reader'
import Widget from '../../../components/Widget';

import s from './venda.module.scss';

import { Api } from '../../../shared/Api.js'

export default class LojistaVenda extends React.Component {

	constructor(props) {
		super(props);

		this.state = { width: 0, height: 0, loading: false, result: 'Pendente de leitura...' };
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

	render() {
		return (
			<div className={s.root}>
				<ol className="breadcrumb">
					<li className="breadcrumb-item">Portal Lojista </li>
					<li className="active breadcrumb-item">
						Identificação do cartão QRCODE
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				<br></br>
				<div align='center'>
					<QrReader
						delay={300}
						onError={this.handleError}
						onScan={this.handleScan}
						style={{ width: '100%' }}
					/>
					<p>{this.state.result}</p>
				</div>
			</div >
		)
	}
}
