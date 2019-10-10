
import React, { createRef } from 'react';
import QRCode from 'qrcode.react';

import {
	Button,
	Col,
	FormGroup,
	Label,
	Input,
} from 'reactstrap';
import Widget from '../../../components/Widget';

import { CircularProgressbar, buildStyles } from "react-circular-progressbar";
import "react-circular-progressbar/dist/styles.css";
import s from './QRCODE.module.scss';

import { Api } from '../../../shared/Api.js'

export default class AssociadoQRCODE extends React.Component {

	constructor(props) {
		super(props);

		var api = new Api();

		this.state = { width: 0, height: 0, loading: false, cartao: api.loggedUserCartao() };
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
				<br></br>				
				<div align='center'>
					<QRCode value={this.state.cartao} size={this.state.width *80/100} /> <br></br>
					<br></br>
					<h3>{this.state.cartao}</h3>
					<br></br>
					<Button color="default"type="submit">Conferir Vendas</Button>
				</div>
			</div >
		)
	}
}
