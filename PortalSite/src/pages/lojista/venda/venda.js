
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
import s from './venda.module.scss';

import { Api } from '../../../shared/Api.js'

export default class LojistaVenda extends React.Component {

	constructor(props) {
		super(props);

		this.state = { width: 0, height: 0, loading: false };
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
					
				</div>
			</div >
		)
	}
}
