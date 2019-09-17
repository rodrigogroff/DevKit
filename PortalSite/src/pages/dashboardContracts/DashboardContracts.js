
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
import Widget from '../../components/Widget';
import Pagination from "react-js-pagination";
import { CircularProgressbar, buildStyles } from "react-circular-progressbar";
import "react-circular-progressbar/dist/styles.css";
import s from './Dashboard.module.scss';

// vortigo components

import filterImg from './filter.png'
import filterXImg from './filterX.png'

import MultiSelectVortigo from '../../components/vortigo/MultiSelectVortigo/MultiSelectVortigo'
import { Api } from '../../shared/Api.js'
import { Util } from '../../shared/Util.js'

export default class DashboardContracts extends React.Component {

	state = {
		loading: false,
		filter: false,
		selectActiveContract: true,
		redirectContract: false,
		totalItemsCount: 0,
		itemsCountPerPage: 8,
		activePage: 1,
		contracts: [],
		editContract: '',
		error: '',
		success: '',
		warning: '',
		searchString: '',
		selectedOption_Country: '',
		dataCountry: [],
		country_list: [],
		selectedOption_State: '',
		dataState: [],
		state_list: [],
		selectedOption_StatusContract: '',
		dataStatusContract: [],
		status_list: [],
		selectedOption_TypeContract: '',
		dataTypeContract: [],
		type_list: [],
		selectedOption_Year: '',
		dataYear: [],
		year_list: [],
	};

	constructor(props) {
		super(props);
		this.nameRef = createRef();
	}

	componentDidMount() {

		var api = new Api();


	}

	loadCombos() {

		var api = new Api();
		var util = new Util();

		let paramAll = 'skip=0&take=100';
		let defaultCountry = 'BRASIL';
		let _allItens = util.getAllItens();

		api.getTokenPortal('typecountry', paramAll).then(resp => {

			var comboData = api.getDropDownItens(resp.payload, _allItens);

			this.setState({
				dataCountry: comboData,
				selectedOption_Country: defaultCountry
			},
				() => {
					this.updateStateList();
				});
		});

		api.getTokenPortal('typestatuscontract', paramAll, _allItens).then(resp => {
			var comboData = api.getDropDownItens(resp.payload, _allItens);
			this.setState({ dataStatusContract: comboData, selectedOption_StatusContract: _allItens })
		});

		api.getTokenPortal('typefinancialcontract', paramAll, _allItens).then(resp => {
			var comboData = api.getDropDownItens(resp.payload, _allItens);
			this.setState({ dataTypeContract: comboData, selectedOption_TypeContract: _allItens })
		});

		var d = new Date();
		var year = d.getFullYear();
		var yearStop = year - 15;
		var year_list = [];
		while (year > yearStop) {
			year_list.push(year.toString())
			year--;
		}
		year_list.push(this.state.allItems);
		this.setState({ dataYear: year_list, selectedOption_Year: _allItens });

		setTimeout(() => {
			this.getContracts();
		}, 500);
	}

	translate = option => {

		var uiTranslation = this.state.languagesArray.languages[option];

		if (uiTranslation !== undefined)
			this.setState({
				resultLabel: uiTranslation.Screens[1].Dashboard.resultLabel,
				contractsLabel: uiTranslation.Screens[1].Dashboard.contractsLabel,
				contractLabel: uiTranslation.Screens[1].Dashboard.contractLabel,
				searchLabel: uiTranslation.Screens[1].Dashboard.searchLabel,
				originCountryLabel: uiTranslation.Screens[1].Dashboard.originCountryLabel,
				stateLabel: uiTranslation.Screens[1].Dashboard.stateLabel,
				contractStatusLabel: uiTranslation.Screens[1].Dashboard.contractStatusLabel,
				typeContractLabel: uiTranslation.Screens[1].Dashboard.typeContractLabel,
				activeContractLabel: uiTranslation.Screens[1].Dashboard.activeContractLabel,
				concludedContractLabel: uiTranslation.Screens[1].Dashboard.concludedContractLabel,
				allContractsLabel: uiTranslation.Screens[1].Dashboard.allContractsLabel,
				localLabel: uiTranslation.Screens[1].Dashboard.localLabel,
				buildingLabel: uiTranslation.Screens[1].Dashboard.buildingLabel,
				equipsLabel: uiTranslation.Screens[1].Dashboard.equipsLabel,
				contractYearLabel: uiTranslation.Screens[1].Dashboard.contractYearLabel,
				resultsLabel: uiTranslation.Screens[1].Dashboard.resultsLabel,
				statusOK: uiTranslation.Screens[1].Dashboard.statusOK,
				statusLate: uiTranslation.Screens[1].Dashboard.statusLate,
				statusActive: uiTranslation.Screens[1].Dashboard.statusActive,
				statusConcluded: uiTranslation.Screens[1].Dashboard.statusConcluded,
				noResultsFound: uiTranslation.Screens[1].Dashboard.noResultsFound,
			}, () => {
				this.loadCombos();
			});
	}

	checkArrayItem = (a, obj) => {
		if (obj.indexOf('(') >= 0 || obj === '')
			return true;
		var i = a.length;
		while (i--)
			if (a[i] === obj)
				return true;
		return false;
	}

	buildInputArray = (lst, selectedItem) => {
		var retStr = '';

		if (lst.length === 0 && selectedItem === '')
			return '';

		if (lst.length > 0) {
			var i = lst.length;
			while (i--)
				retStr += lst[i] + ','
		}
		else
			retStr = selectedItem;

		if (retStr === '')
			retStr = this.state.allItems;

		return retStr
	}

	openCloseFilter = event => {
		this.setState({ filter: !this.state.filter });
	}

	enterSearchString = event => {
		if (event.key === 'Enter')
			if (event.target.value === '')
				this.setState({ activePage: 1 }, () => { this.getContracts(); })
			else
				this.getContracts();
	}

	redirToContract = e => {
		this.setState({ redirectContract: true, editContract: '/app/main/contract?id=' + e })
	}

	/* ------- Dropdown INICIO ------------------  */

	/* ------- Dropdown Country Multi-Select ------------------  */

	changeSelectCountry = e => { this.setState({ selectedOption_Country: e.currentTarget.textContent }, () => { this.updateStateList() }); }
	addSelectedCountry = e => {
		var lst = this.state.country_list;
		var _target = this.state.selectedOption_Country;
		if (!this.checkArrayItem(lst, _target)) {
			lst.push(_target);
			this.setState({ country_list: lst });
		}
		this.setState({ selectedOption_Country: '' });
	}
	removeCountryItem = selectedItem => {
		var lst = this.state.country_list.filter(item => item !== selectedItem);
		this.setState({ country_list: lst })
		if (lst.length === 0) {
			var util = new Util();
			this.setState({ selectedOption_Country: util.getAllItens() });
		}
	}

	/* ------- Dropdown State Multi-Select ------------------  */

	updateStateList = e => {
		var util = new Util();
		var todos_Str = util.getAllItens();
		if (this.state.selectedOption_Country.indexOf('(') !== 0) {
			var api = new Api();
			api.getTokenPortal('typestate', 'country=' + this.state.selectedOption_Country + '&skip=0&take=99').then(resp => {
				var comboData = api.getDropDownItensTag(resp.payload, todos_Str);
				this.setState({ dataState: comboData, selectedOption_State: todos_Str })
			});
		}
	}
	changeSelectState = e => { this.setState({ selectedOption_State: e.currentTarget.textContent }); }
	addSelectedState = e => {
		var lst = this.state.state_list;
		var _target = this.state.selectedOption_State;
		if (!this.checkArrayItem(lst, _target)) {
			lst.push(_target);
			this.setState({ state_list: lst, selectedOption_State: '' });
		}
		this.setState({ selectedOption_State: '' });
	}
	removeStateItem = selectedItem => {
		var lst = this.state.state_list.filter(item => item !== selectedItem);
		this.setState({ state_list: lst })
		if (lst.length === 0) {
			var util = new Util();
			this.setState({ selectedOption_State: util.getAllItens() });
		}
	}

	/* ------- Dropdown Status Multi-Select ------------------  */

	changeSelectStatus = e => { this.setState({ selectedOption_StatusContract: e.currentTarget.textContent }); }
	addSelectedStatus = e => {
		var lst = this.state.status_list;
		var _target = this.state.selectedOption_StatusContract;
		if (!this.checkArrayItem(lst, _target)) {
			lst.push(_target);
			this.setState({ status_list: lst });
		}
		this.setState({ selectedOption_StatusContract: '' });
	}
	removeStatusItem = selectedItem => {
		var lst = this.state.status_list.filter(item => item !== selectedItem);
		this.setState({ status_list: lst })
		if (lst.length === 0) {
			var util = new Util();
			this.setState({ selectedOption_StatusContract: util.getAllItens() });
		}
	}

	/* ------- Dropdown Type Multi-Select ------------------  */

	changeSelectType = e => { this.setState({ selectedOption_TypeContract: e.currentTarget.textContent }); }
	addSelectedType = e => {
		var lst = this.state.type_list;
		var _target = this.state.selectedOption_TypeContract;
		if (!this.checkArrayItem(lst, _target)) {
			lst.push(_target);
			this.setState({ type_list: lst });
		}
		this.setState({ selectedOption_TypeContract: '' });
	}
	removeTypeItem = selectedItem => {
		var lst = this.state.type_list.filter(item => item !== selectedItem);
		this.setState({ type_list: lst })
		if (lst.length === 0) {
			var util = new Util();
			this.setState({ selectedOption_TypeContract: util.getAllItens() });
		}
	}

	/* ------- Dropdown Status Multi-Select ------------------  */

	changeSelectYear = e => { this.setState({ selectedOption_Year: e.currentTarget.textContent }); }
	addSelectedYear = e => {
		var lst = this.state.year_list;
		var _target = this.state.selectedOption_Year;
		if (!this.checkArrayItem(lst, _target)) {
			lst.push(_target);
			this.setState({ year_list: lst });
		}
		this.setState({ selectedOption_Year: '' });
	}
	removeYearItem = selectedItem => {
		var lst = this.state.year_list.filter(item => item !== selectedItem);
		this.setState({ year_list: lst })
		if (lst.length === 0) {
			var util = new Util();
			this.setState({ selectedOption_Year: util.getAllItens() });
		}
	}

	/* ------- Dropdown FIM ------------------  */

	changeActiveTrue = () => {
		if (!this.state.loading)
			this.setState({ selectActiveContract: true }, () => { this.getContracts() })
	}

	changeActiveFalse = () => {
		if (!this.state.loading)
			this.setState({ selectActiveContract: false }, () => { this.getContracts() })
	}

	changeActiveNull = () => {
		if (!this.state.loading)
			this.setState({ selectActiveContract: null }, () => { this.getContracts() })
	}

	handlePageChange = pageNumber => {
		this.setState({ activePage: pageNumber },
			() => {
				this.getContracts();
				setTimeout(() => { this.nameRef.current.focus(); }, 500);
			})
	}

	getName = () => {
		return localStorage.getItem('user_nameFull')
	}

	searchButtonClick = e => {

		this.setState({ activePage: 1 },
			() => {
				this.getContracts();
			})
	}

	getContracts = () => {

		this.setState({ loading: true, error: '' });

		var api = new Api();

		var paramData = 'active=' + (this.state.selectActiveContract === true ? 'true' : this.state.selectActiveContract === false ? 'false' : '') +
			'&search=' + this.state.searchString +
			'&country=' + this.buildInputArray(this.state.country_list, this.state.selectedOption_Country) +
			'&state=' + this.buildInputArray(this.state.state_list, this.state.selectedOption_State) +
			'&status=' + this.buildInputArray(this.state.status_list, this.state.selectedOption_StatusContract) +
			'&typeContract=' + this.buildInputArray(this.state.type_list, this.state.selectedOption_TypeContract) +
			'&year=' + this.buildInputArray(this.state.year_list, this.state.selectedOption_Year) +
			'&skip=' + (this.state.itemsCountPerPage * (Number(this.state.activePage) - 1)) +
			'&take=' + this.state.itemsCountPerPage

		api.getTokenPortal('dashboardContracts', paramData).then(resp => {
			if (resp.ok === false) {
				this.setState({
					loading: false,
					error: resp.msg,
				});
			}
			else
				this.setState({
					loading: false,
					totalItemsCount: resp.payload.total,
					contracts: resp.payload.results,
					warning: resp.payload.results.length === 0 ? this.state.noResultsFound : ''
				});

			if (resp.payload.results.length === 1 && this.state.searchString.length > 0)
				this.setState({ redirectContract: true, editContract: '/app/main/contract?id=' + this.state.searchString })
		});
	}

	render() {
		if (this.state.redirectContract === true)
			return <Redirect to={this.state.editContract} />
		else
			return (
				<div className={s.root}>
					<Modal isOpen={this.state.success.length > 0} toggle={() => this.setState({ success: '' })}>
						<ModalHeader toggle={() => this.setState({ success: '' })}>Aviso do Sistema</ModalHeader>
						<ModalBody className="bg-success-system"><div className="modalBodyMain"><br></br>{this.state.success}<br></br><br></br></div></ModalBody>
						<ModalFooter className="bg-white"><Button color="primary" onClick={() => this.setState({ success: '' })}>Fechar</Button></ModalFooter>
					</Modal>
					<Modal isOpen={this.state.error.length > 0} toggle={() => this.setState({ error: '' })}>
						<ModalHeader toggle={() => this.setState({ error: '' })}>Aviso do Sistema</ModalHeader>
						<ModalBody className="bg-danger-system"><div className="modalBodyMain"><br></br>{this.state.error}<br></br><br></br></div></ModalBody>
						<ModalFooter className="bg-white"><Button color="primary" onClick={() => this.setState({ error: '' })}>Fechar</Button></ModalFooter>
					</Modal>
					{this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
					<FormGroup row>
						<Col xs={12} sm={12} lg={12} xl={12}>
							<br></br>
							<h4>Seja bem-vindo, {this.getName()}</h4>
							<br></br>
							<table width='100%'>
								<tbody>
									<tr>
										<td width="100%">
											<Input className="input-transparent"
												innerRef={this.nameRef}
												type="text"
												id="fieldName"
												maxLength="50"
												placeholder="Digite o número do contrato"
												value={this.state.searchString}
												onChange={event => this.setState({ searchString: event.target.value })}
												onKeyDown={this.enterSearchString} />
										</td>
										<td width='8px'></td>
										<td>
											<Button color="primary" onClick={this.searchButtonClick}>{this.state.searchLabel}</Button>
										</td>
										<td width='8px'></td>
										<td>
											<img title='Filtros' alt=" " className={s.mousePointer} src={this.state.filter ? filterXImg : filterImg} onClick={this.openCloseFilter} />
										</td>
									</tr>
								</tbody>
							</table>
							{
								this.state.filter === true ? <div>
									<br></br>
									<FormGroup row>
										<Col xs={12} sm={6} lg={6} xl={2}>
											<FormGroup>
												<Col >
													<Label for="normal-field" className="text-md-left"><span className={s.lblTitle}>{this.state.originCountryLabel}</span></Label>
												</Col>
												<Col>
													<MultiSelectVortigo selectedOption={this.state.selectedOption_Country}
														items={this.state.dataCountry}
														changeSelect={this.changeSelectCountry}
														selected_list={this.state.country_list}
														addSelectedItem={this.addSelectedCountry}
														removeItem={this.removeCountryItem} />
												</Col>
											</FormGroup>
										</Col>
										<Col xs={12} sm={6} lg={6} xl={2}>
											<FormGroup>
												<Col>
													<Label for="normal-field" className="text-md-left"><span className={s.lblTitle}>{this.state.stateLabel}</span></Label>
												</Col>
												<Col>
													<MultiSelectVortigo selectedOption={this.state.selectedOption_State}
														items={this.state.dataState}
														changeSelect={this.changeSelectState}
														selected_list={this.state.state_list}
														addSelectedItem={this.addSelectedState}
														removeItem={this.removeStateItem} />
												</Col>
											</FormGroup>
										</Col>
										<Col xs={12} sm={6} lg={6} xl={2}>
											<FormGroup>
												<Col>
													<Label for="normal-field" className="text-md-left"><span className={s.lblTitle}>{this.state.contractStatusLabel}</span></Label>
												</Col>
												<Col>
													<MultiSelectVortigo selectedOption={this.state.selectedOption_StatusContract}
														items={this.state.dataStatusContract}
														changeSelect={this.changeSelectStatus}
														selected_list={this.state.status_list}
														addSelectedItem={this.addSelectedStatus}
														removeItem={this.removeStatusItem} />
												</Col>
											</FormGroup>
										</Col>
										<Col xs={12} sm={6} lg={6} xl={2}>
											<FormGroup>
												<Col>
													<Label for="normal-field" className="text-md-left"><span className={s.lblTitle}>{this.state.contractYearLabel}</span></Label>
												</Col>
												<Col>
													<MultiSelectVortigo selectedOption={this.state.selectedOption_Year}
														items={this.state.dataYear}
														changeSelect={this.changeSelectYear}
														selected_list={this.state.year_list}
														addSelectedItem={this.addSelectedYear}
														removeItem={this.removeYearItem} />
												</Col>
											</FormGroup>
										</Col>
										<Col xs={12} sm={6} lg={6} xl={2}>
											<FormGroup>
												<Col>
													<Label for="normal-field" className="text-md-left"><span className={s.lblTitle}>{this.state.typeContractLabel}</span></Label>
												</Col>
												<Col>
													<MultiSelectVortigo selectedOption={this.state.selectedOption_TypeContract}
														items={this.state.dataTypeContract}
														changeSelect={this.changeSelectType}
														selected_list={this.state.type_list}
														addSelectedItem={this.addSelectedType}
														removeItem={this.removeTypeItem} />
												</Col>
											</FormGroup>
										</Col>
									</FormGroup>
								</div>
									: <div></div>
							}
						</Col>
						<Col xs={12} sm={12} lg={12} xl={12}>
							<FormGroup row>
								<Col xs={12} sm={12} lg={12} xl={12}>
									<br></br>
									<FormGroup>
										<Col>
											<Label for="normal-field" className="text-md-left"><span className={s.lblTitle}>Situação dos contratos</span></Label>
										</Col>
										<Col>
											<span className={this.state.selectActiveContract === true ? "btn btn-primary" : this.state.selectActiveContract === false ? s.lblInactive : s.lblInactive} onClick={this.changeActiveTrue}>{this.state.activeContractLabel} </span> &nbsp;&nbsp;&nbsp;&nbsp;
                                                                <span className={this.state.selectActiveContract === true ? s.lblInactive : this.state.selectActiveContract === false ? "btn btn-primary" : s.lblInactive} onClick={this.changeActiveFalse}>{this.state.concludedContractLabel}</span> &nbsp;&nbsp;&nbsp;&nbsp;
                                                                <span className={this.state.selectActiveContract === null ? "btn btn-primary" : s.lblInactive} onClick={this.changeActiveNull}>{this.state.allContractsLabel}</span> &nbsp;&nbsp;&nbsp;&nbsp;
                                                            </Col>
									</FormGroup>
								</Col>
							</FormGroup>
							<FormGroup row>
								<Col xs={12} sm={12} lg={12} xl={12}>
									<table width='100%'>
										<tbody>
											<tr>
												<td width='50%'>
													<h4>{this.state.totalItemsCount} {this.state.resultLabel}</h4>
												</td>
												<td className={s.cardRight}>
													<Pagination hideNavigation
														innerClass={s.pagination_innerClass}
														activeClass={s.pagination_activeClass}
														activeLinkClass={s.pagination_activeLinkClass}
														itemClass={s.pagination_itemClass}
														itemClassFirst={s.pagination_itemClass}
														itemClassPrev={s.pagination_itemClass}
														itemClassNext={s.pagination_itemClass}
														linkClass={s.pagination_linkClass}
														pageRangeDisplayed={10}
														activePage={this.state.activePage}
														itemsCountPerPage={this.state.itemsCountPerPage}
														totalItemsCount={this.state.totalItemsCount}
														pageRangeDisplayed={5}
														onChange={this.handlePageChange}
													/>
												</td>
											</tr>
										</tbody>
									</table>
								</Col>
							</FormGroup>
						</Col>
					</FormGroup>
					<FormGroup row style={{ visibility: this.state.error === '' ? 'visible' : 'hidden' }}>
						{this.state.contracts.map((current, index) => (
							<Col xs={12} sm={6} lg={4} xl={3} key={`${index}`}>
								<Widget title={
									<div>
										<div className={s.cardRight}><p>{current.dateFinish}</p></div>
										<div >
											<p>{current.active === 'true' ? this.state.statusActive : this.state.statusConcluded} / {current.status}</p></div>
									</div>}>
									<div align="center"
										className={s.mousePointer}
										onClick={() => this.redirToContract(current.contractId)}>
										<Col xs={9} sm={9} lg={9} xl={9}>
											<CircularProgressbar
												value={current.pct} text={`${current.pct}%`}
												background counterClockwise backgroundPadding={6}
												styles={buildStyles({
													backgroundColor: "#3a3a3a",
													pathColor: "#ffffff",
													textColor: "#fff",
													trailColor: '#708294',
												})} />
										</Col>
										<br></br>
										<h4>{this.state.contractLabel} {current.contractId}</h4>
										{current.buildingName} <br></br>
										{current.country} / {current.address} {current.state} <br></br>
										{current.typeContract}<br></br>
										{current.qttyEquip} {this.state.equipsLabel}</div>
									<br></br>
									<table width='100%'>
										<tbody>
											<tr>
												<td width='50%'>
													<Button color="primary" className="btn-rounded-f btn-block">Boletos</Button>
												</td>
												<td width='8px'></td>
												<td>
													<Button color="primary" className="btn-rounded-f btn-block">Notas Fiscais</Button>
												</td>
											</tr>
										</tbody>
									</table>
								</Widget>
							</Col>
						))}
					</FormGroup>
					{
						this.state.contracts.length > 4 ?
							<div>
								<FormGroup row>
									<Col xs={12} sm={6} lg={6} xl={6}>
										<h4>{this.state.totalItemsCount} {this.state.resultLabel}</h4>
									</Col>
									<Col xs={12} sm={6} lg={6} xl={6}>
										<div className={s.paginationMain}>
											<Pagination hideNavigation
												innerClass={s.pagination_innerClass} activeClass={s.pagination_activeClass} activeLinkClass={s.pagination_activeLinkClass}
												itemClass={s.pagination_itemClass} itemClassFirst={s.pagination_itemClass} itemClassPrev={s.pagination_itemClass}
												itemClassNext={s.pagination_itemClass} linkClass={s.pagination_linkClass} pageRangeDisplayed={10}
												activePage={this.state.activePage} itemsCountPerPage={this.state.itemsCountPerPage} totalItemsCount={this.state.totalItemsCount}
												pageRangeDisplayed={5} onChange={this.handlePageChange} />
										</div>
									</Col>
								</FormGroup>
							</div>
							:
							<div></div>
					}
				</div >
			)
	}
}
