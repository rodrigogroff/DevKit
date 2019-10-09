
import React, { createRef } from 'react';
import QRCode from 'qrcode.react';

import {
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

	state = {
		loading: false,		
	};

	componentDidMount() {
		
	}

	render() {
		return (
			<div className={s.root}>
				<ol className="breadcrumb">
					<li className="breadcrumb-item">Portal </li>
					<li className="active breadcrumb-item">
						Identificação do cartão 
						{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					</li>
				</ol>
				<QRCode value="http://facebook.github.io/react/" />
			</div >
		)
	}
}
