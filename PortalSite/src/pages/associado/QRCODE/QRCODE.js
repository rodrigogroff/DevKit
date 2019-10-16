
import React from 'react';
import QRCode from 'qrcode.react';

import {
	Button,
} from 'reactstrap';

import s from './QRCODE.module.scss';

import { Api } from '../../../shared/Api.js'

import Background from "./cnet_virtual.png";



export default class AssociadoQRCODE extends React.Component {

	constructor(props) {
		super(props);

		var api = new Api();

		this.state = {
			width: 0,
			height: 0,
			loading: false,
			cartao: api.loggedUserCartao(),
			nome: api.loggedUserName()
		};

		this.updateWindowDimensions = this.updateWindowDimensions.bind(this);
	}

	componentDidMount() {
		this.updateWindowDimensions();
		window.addEventListener('resize', this.updateWindowDimensions);
	}

	componentWillUnmount() {
		window.removeEventListener('resize', this.updateWindowDimensions);
	}

	updateWindowDimensions() {
		this.setState({ width: window.innerWidth, height: window.innerHeight });
	}

	render() {
		return (
			<div className={s.root}>
				<ol className="breadcrumb">
					<li className="breadcrumb-item">Portal </li>
					<li className="active breadcrumb-item">
						Identificação do cartão QRCODE
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				<table width='100%' style={{
					backgroundImage: "url(" + Background + ")",
					backgroundPosition: 'top',
					backgroundSize: 'contain',
					backgroundRepeat: 'no-repeat'
				}}>
					<tbody>
						<tr>
							<td align='center' valign='top'>
								<br></br>
								<br></br>
								<br></br>
								<br></br>
								<QRCode value={this.state.cartao} size={this.state.width * 30 / 100} /> <br></br>
								<br></br>
								<br></br>
							</td>
						</tr>
						<tr>
							<td align='center'>

							</td>
						</tr>
					</tbody>
				</table>
				<div align='center'>
					<h4>{this.state.nome}</h4>
					<h3>{this.state.cartao}</h3>
					<Button color="success" type="submit">Conferir Vendas</Button>
				</div>
				<br></br>
				<br></br>
				<br></br>
				<br></br>
			</div >
		)
	}
}

