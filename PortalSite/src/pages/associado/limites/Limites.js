
import React, { createRef } from 'react';
import { Redirect } from 'react-router';
import {
	Col,
	Button,
	FormGroup,
	Label,
	Modal,
	ModalHeader,
	ModalBody,
	ModalFooter,
	Input,
} from 'reactstrap';
import Widget from '../../../components/Widget';
import Pagination from "react-js-pagination";
import { CircularProgressbar, buildStyles } from "react-circular-progressbar";
import "react-circular-progressbar/dist/styles.css";
import s from './Limites.module.scss';

import filterImg from './filter.png'
import filterXImg from './filterX.png'

import { Api } from '../../../shared/Api.js'


export default class AssociadoLimites extends React.Component {

	state = {

	};

	componentDidMount() {
	}

	getName = () => {
		return localStorage.getItem('user_nameFull')
	}

	render() {
		//		if (this.state.redirectContract === true)
		//			return <Redirect to={this.state.editContract} />
		//		else
		return (
			<div className={s.root}>
				<h2>Limites do associado</h2>
			</div>
		)
	}
}
