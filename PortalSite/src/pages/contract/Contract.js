
import React from 'react';
import { Redirect } from 'react-router';
import { withRouter } from 'react-router-dom';
import {
  Col,
  Button,
  FormGroup,
  Popover,
  PopoverBody,
  Label,
  Tooltip,
} from 'reactstrap';
import Widget from "../../components/Widget";
import { CircularProgressbar, buildStyles } from "react-circular-progressbar";
import "react-circular-progressbar/dist/styles.css";
import queryString from 'query-string';

import s from './Contract.module.scss';

// charts -----------------

/* eslint-disable */
import 'imports-loader?jQuery=jquery,this=>window!flot';
import 'imports-loader?jQuery=jquery,this=>window!flot/jquery.flot.time';
import 'imports-loader?jQuery=jquery,this=>window!flot/jquery.flot.resize';
import 'imports-loader?jQuery=jquery,this=>window!jquery.flot.animator/jquery.flot.animator';
import 'imports-loader?jQuery=jquery,this=>window!flot/jquery.flot.crosshair';
import 'imports-loader?jQuery=jquery,this=>window!flot/jquery.flot.symbol';
import 'imports-loader?jQuery=jquery,this=>window!flot/jquery.flot.selection';
import 'imports-loader?jQuery=jquery,this=>window!flot/jquery.flot.pie';
import 'imports-loader?jQuery=jquery,this=>window!jquery.flot-orderBars/js/jquery.flot.orderBars';
import 'imports-loader?jQuery=jquery,this=>window!jquery-sparkline';
/* eslint-enable */
import TrackingChart from './charts/TrackingChart';

// vortigo components -----------------

import MultiSelectVortigo from '../../components/vortigo/MultiSelectVortigo/MultiSelectVortigo'
import { Api } from '../../shared/Api.js'
import { Util } from '../../shared/Util.js'

import majorImg from "./major.png";
import minorImg from "./minor.png";
import checkImg from "./check.png";

class ContractDetailPage extends React.Component {

  state = {
    loading: false,
    redirectDashboard: false,
    steps: ['1', '2', '3', '4', '5'],
    currentStepPopoverText: [''],
    stepPopover: [false, false, false, false, false],
    currentContract: '',
    allContracts: [],
    allContractsText: [],
    popoverHelp: [],
    popoverDesc: [],
    buildings: [],
    stepsDefinition: [],
    totEquips: 0,
    selectedOption_Building: '',
    dataBuilding: [],
    building_list: [],
    selectedOption_Equipment: '',
    dataEquipment: [],
    equipment_list: [],
    contractId: '',
    error: '',
    warning: '',
    currentPopoverText: [''],
  };

  /* ------- Page Setup ------------------  */

  componentDidMount() {

    var api = new Api();


  }

  /* ------- Multi-Language ------------------  */

  translate = option => {

    var uiTranslation = this.state.languagesArray.languages[option];

    if (uiTranslation !== undefined)
      this.setState({
        allItens: uiTranslation.Screens[1].Dashboard.allItens,
      }, () => {
        var _params = queryString.parse(this.props.location.search);
        this.getContractDetails(_params.id);
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
        retStr += lst[i] + '|'
    }
    else
      retStr = selectedItem;

    if (retStr === '')
      retStr = this.state.allItems;

    return retStr
  }

  /*  ------- UI Events ------------------  */

  returnDashboard = e => {
    this.setState({ redirectDashboard: true })
  }

  /* ------- Dropdown Building Multi-Select ------------------  */

  changeSelectBuilding = e => { this.setState({ selectedOption_Building: e.currentTarget.textContent }, () => { }); }
  addSelectedBuilding = e => {
    var lst = this.state.building_list;
    var _target = this.state.selectedOption_Building;
    if (!this.checkArrayItem(lst, _target)) {
      lst.push(_target);
      this.setState({ building_list: lst });
    }
    this.setState({ selectedOption_Building: '' });
  }
  removeBuildingItem = selectedItem => {
    var lst = this.state.building_list.filter(item => item !== selectedItem);
    this.setState({ building_list: lst })
    if (lst.length === 0) {
      var util = new Util();
      this.setState({ selectedOption_Building: util.getAllItens() });
    }
  }

  /* ------- Dropdown Equipment Multi-Select ------------------  */

  changeSelectEquipment = e => { this.setState({ selectedOption_Equipment: e.currentTarget.textContent }, () => { }); }
  addSelectedEquipment = e => {
    var lst = this.state.equipment_list;
    var _target = this.state.selectedOption_Equipment;
    if (!this.checkArrayItem(lst, _target)) {
      lst.push(_target);
      this.setState({ equipment_list: lst });
    }
    this.setState({ selectedOption_Equipment: '' });
  }
  removeEquipmentItem = selectedItem => {
    var lst = this.state.equipment_list.filter(item => item !== selectedItem);
    this.setState({ equipment_list: lst })
    if (lst.length === 0) {
      var util = new Util();
      this.setState({ selectedOption_Equipment: util.getAllItens() });
    }
  }

  toggleMain = (step) => {
    var sp = this.state.stepPopover;
    sp[step] = !sp[step]
    this.setState({
      stepPopover: sp,
      currentStepPopoverText: this.state.stepsDefinition[step - 1].steps,
    });
  }

  toggleHelp = (step) => {
    var newPopover = this.state.popoverHelp;
    newPopover[step._id] = !newPopover[step._id];
    this.setState({
      popoverHelp: newPopover,
      currentPopoverText: this.state.stepsDefinition[step._step - 1].steps,
    });
  }

  toggleDesc = (step) => {
    var newPopover = this.state.popoverDesc;
    newPopover[step._id] = !newPopover[step._id];
    this.setState({
      popoverDesc: newPopover,
    });
  }

  clickPesquisar = () => {
    this.setState({ buildings: [], refresh: true });
    var target = this.state.currentContract;
    var num = target.substring(target.indexOf(' '))
    this.getContractDetails(num);
  }

  /* ------- Dropdown Contracts Multi-Select ------------------  */

  changeSelectContract = e => {
    this.setState({ currentContract: e.currentTarget.textContent });
    var target = e.currentTarget.textContent;
    var num = target.substring(target.indexOf(' '))
    this.getContractDetails(num);
  }

  incrementContract = () => {
    var cc = this.state.currentContract;
    var pos = 0;
    for (let i = 0; i < this.state.allContractsText.length; i++) {
      if (cc === this.state.allContractsText[i]) {
        pos = i;
        break;
      }
    }
    pos++;
    if (pos >= this.state.allContracts.length)
      pos = 0;
    this.setState({ currentContract: this.state.allContractsText[pos] });
    this.getContractDetails(this.state.allContracts[pos]);
  }

  decrementContract = () => {
    var cc = this.state.currentContract;
    var pos = 0;
    for (let i = 0; i < this.state.allContractsText.length; i++) {
      if (cc === this.state.allContractsText[i]) {
        pos = i;
        break;
      }
    }
    pos--;
    if (pos < 0)
      pos = this.state.allContracts.length - 1;
    this.setState({ currentContract: this.state.allContractsText[pos] });
    this.getContractDetails(this.state.allContracts[pos]);
  }

  /* ------- Api Interface (Clients) ------------------  */

  getContractDetails = id => {

    this.setState({ loading: true, error: '' });

    var api = new Api();

    var paramData = 'contract=' + id +
      '&building=' + this.buildInputArray(this.state.building_list, this.state.selectedOption_Building) +
      '&equip=' + this.buildInputArray(this.state.equipment_list, this.state.selectedOption_Equipment);

    api.getTokenPortal('contractDetail', paramData).then(resp => {
      if (resp.ok === false) {
        this.setState({
          loading: false,
          error: resp.msg,
        });
      }
      else {
        this.setState({
          loading: false,
          buildings: [],
        });

        var isSetupPhase = this.state.dataBuilding.length === 0

        let _allItens = this.state.allItens;

        var comboDataBuilding = [],
          comboDataEquipment = [],
          popoverHelpItem = [],
          popoverDescItem = []

        let _totEquips = 0, _totSteps = 0;

        for (let i = 0; i < resp.payload.buildings.length; ++i) {
          var b = resp.payload.buildings[i];
          if (isSetupPhase)
            comboDataBuilding.push(b.buildingName);
          for (let j = 0; j < b.equips.length; ++j) {
            var c = b.equips[j];
            _totEquips++;
            for (let x = 0; x < c.steps.length; x++) {
              var d = c.steps[x];
              d._id = _totSteps;
              d._step = x + 1;
              popoverHelpItem.push(false);
              popoverDescItem.push(false);
              _totSteps++;
            }
            if (isSetupPhase)
              comboDataEquipment.push(b.equips[j].equipmentName);
          }
        }

        if (isSetupPhase) {
          comboDataBuilding.push(_allItens);
          comboDataEquipment.push(_allItens);
        }

        var detail = {
          nameClient: resp.payload.nameClient,
          pct: resp.payload.pct,
          dtConclusion: resp.payload.dtConclusion,
          country: resp.payload.country,
          state: resp.payload.state,
          status: resp.payload.status,
          typeEquip: resp.payload.typeEquip,
        }

        var _allContractsText = [];

        for (let i = 0; i < resp.payload.allContracts.length; i++)
          _allContractsText.push('Contrato ' + resp.payload.allContracts[i])

        this.setState({
          allContracts: resp.payload.allContracts,
          allContractsText: _allContractsText,
          currentContract: 'Contrato ' + id,
          totEquips: _totEquips,
          popoverHelp: popoverHelpItem,
          popoverDesc: popoverDescItem,
          contractDetail: detail,
          buildings: resp.payload.buildings,
          stepsDefinition: resp.payload.stepsSetup,
          dataBuilding: isSetupPhase ? comboDataBuilding : this.state.dataBuilding,
          dataEquipment: isSetupPhase ? comboDataEquipment : this.state.dataEquipment,
          selectedOption_Building: isSetupPhase ? _allItens : this.state.selectedOption_Building,
          selectedOption_Equipment: isSetupPhase ? _allItens : this.state.selectedOption_Equipment,
        })
      }
    });
  }

  /* ------- Display ------------------  */

  render() {

    var contractDet = this.state.contractDetail;

    if (this.state.redirectDashboard === true)
      return <Redirect to='/app/main/dashboard' />
    else
      return (
        <div className={s.root}>
          {this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}
          {
            this.state.buildings.length > 0 ?
              <div>
                <br></br>
                <ol className="breadcrumb">
                  <li onClick={this.returnDashboard}
                    className="breadcrumb-item">Dashboard</li>
                  <li className="active breadcrumb-item">{this.state.currentContract} </li>
                </ol>
                <MultiSelectVortigo size='lg'
                  selectedOption={this.state.currentContract}
                  items={this.state.allContractsText}
                  changeSelect={this.changeSelectContract} />
                <br></br>
                <br></br>
                <FormGroup row>
                  <Col md={12}>
                    <FormGroup row>
                      <Col xs={12} sm={3} lg={3} xl={3}>
                        <table width='100%'>
                          <tbody>
                            <tr>
                              <td valign='top'>
                                <br></br><br></br><br></br>
                                <img src={minorImg} alt=" " className={s.mousePointer} onClick={this.decrementContract} title='Contrato anterior'></img>
                              </td>
                              <td width='20px'></td>
                              <td valign='top'>
                                <div className={s.ballPctBig}>
                                  <CircularProgressbar
                                    value={contractDet.pct} text={contractDet.pct + '%'}
                                    background counterClockwise backgroundPadding={6}
                                    styles={buildStyles({
                                      backgroundColor: "#3a3a3a",
                                      textColor: "#ffffff",
                                      pathColor: "#fff",
                                      trailColor: "#708294"
                                    })} />
                                  <br></br>
                                  <div align='center'>
                                    <br></br>
                                    <h4>{contractDet.status}</h4>
                                    <p>Data de Conclusão: {contractDet.dtConclusion}</p>
                                  </div>
                                  <br></br>
                                </div>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </Col>
                      <Col xs={12} sm={6} lg={4} xl={4}>
                        <div className={s.leftDiv}>
                          <table width='100%'>
                            <tbody>
                              <tr height='55px'>
                                <td width='50%'>
                                  <h4>{contractDet.country}</h4>
                                </td>
                                <td>
                                  <h4 className={s.rightDiv}>{contractDet.typeEquip}</h4>
                                </td>
                              </tr>
                              <tr height='55px'>
                                <td>
                                  <h4>UF: {contractDet.state}</h4>
                                </td>
                                <td>
                                  <h4 className={s.rightDiv}>Equipamentos: {this.state.totEquips}</h4>
                                </td>
                              </tr>
                              <tr height='55px'>
                                <td>
                                  <p>{contractDet.nameClient}</p>
                                </td>
                                <td>
                                  <h4 className={s.rightDiv}>Edificios: {this.state.buildings.length}</h4>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                      </Col>
                      <Col xs={12} sm={3} lg={4} xl={5}>
                        <div className={s.rightDiv}>
                          <table width='100%'>
                            <tbody>
                              <tr>
                                <td>
                                  <Button color="dark" className={"btn-rounded-f my-2"} style={{ width: "100%" }}>Documentos</Button>
                                  <Button color="dark" className={"btn-rounded-f my-2"} style={{ width: "100%" }}>Boletos</Button>
                                  <Button color="dark" className={"btn-rounded-f my-2"} style={{ width: "100%" }}>Notas Fiscais</Button>
                                </td>
                                <td width='20px'></td>
                                <td valign='top'>
                                  <br></br><br></br><br></br>
                                  <img src={majorImg} alt=" " className={s.mousePointer} onClick={this.incrementContract} title='Contrato posterior'></img>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                      </Col>
                    </FormGroup>
                  </Col>
                  <Col xs={12} sm={12} lg={12} xl={12}>
                    <div className={s.rowNegLeft}>
                      <FormGroup row>
                        <Col xs={12} sm={2} lg={2} xl={1}>
                          <FormGroup>
                            <Col>
                              <br></br>
                              <Button color="primary" onClick={this.clickPesquisar}>Pesquisar</Button>
                            </Col>
                          </FormGroup>
                        </Col>
                        <Col xs={12} sm={5} lg={4} xl={3}>
                          <FormGroup>
                            <Col>
                              <Label for="normal-field" className="text-md-left"><span className={s.lblTitle}>Edificios ({this.state.buildings.length})</span></Label>
                            </Col>
                            <Col>
                              <MultiSelectVortigo selectedOption={this.state.selectedOption_Building}
                                items={this.state.dataBuilding}
                                changeSelect={this.changeSelectBuilding}
                                selected_list={this.state.building_list}
                                addSelectedItem={this.addSelectedBuilding}
                                removeItem={this.removeBuildingItem} />
                            </Col>
                          </FormGroup>
                        </Col>
                        <Col xs={12} sm={4} lg={4} xl={3}>
                          <FormGroup>
                            <Col>
                              <Label for="normal-field" className="text-md-left"><span className={s.lblTitle}>Equipamentos ({this.state.totEquips})</span></Label>
                            </Col>
                            <Col>
                              <MultiSelectVortigo selectedOption={this.state.selectedOption_Equipment}
                                items={this.state.dataEquipment}
                                changeSelect={this.changeSelectEquipment}
                                selected_list={this.state.equipment_list}
                                addSelectedItem={this.addSelectedEquipment}
                                removeItem={this.removeEquipmentItem} />
                            </Col>
                          </FormGroup>
                        </Col>
                      </FormGroup>
                    </div>
                  </Col>
                </FormGroup>
                <Widget title={<legend>Detalhes dos Equipamentos</legend>}>
                  <table width='100%' height='100px'>
                    <tbody>
                      <tr>
                        <td width='20%'></td>
                        <td width='80%'>
                          <table width='100%' height='100px'>
                            <tbody>
                              <tr>
                                {this.state.steps.map((step, index) => (
                                  <td width='20%' key={`${index}`}>
                                    <div align='center'>
                                      <h4>Etapa {step} </h4>
                                      <Button color="gray" size="sm" title="detalhes da etapa"
                                        id={`stepdesc_${step}`}
                                        onClick={() => this.toggleMain(step)}><i className='fa fa-plus' /></Button>
                                    </div>
                                    <Popover placement="bottom"
                                      isOpen={this.state.stepPopover[step]}
                                      target={`stepdesc_${step}`}
                                      toggle={() => this.toggleMain(step)}>>
                                      <PopoverBody>
                                        ETAPA {step._step}: <br></br>
                                        <br></br>
                                        Nesta etapa teremos:<br></br>
                                        <br></br>
                                        {this.state.currentStepPopoverText.map((stepDesc, index) => (
                                          <table key={`${index}`}>
                                            <tbody>
                                              <tr>
                                                <td><i className='fa fa-circle' /></td>
                                                <td width='8px'></td>
                                                <td>{stepDesc}</td>
                                              </tr>
                                            </tbody>
                                          </table>
                                        ))}
                                        <br></br>
                                      </PopoverBody>
                                    </Popover>
                                  </td>
                                ))}
                              </tr>
                            </tbody>
                          </table>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                  {this.state.buildings.map((currentBuilding, index) => (
                    <div key={`${index}`}>
                      <br></br>
                      <h1 style={{ paddingLeft: '20px' }}>{currentBuilding.buildingName}</h1>
                      <table width='100%'>
                        <tbody>
                          {currentBuilding.equips.map((equip, index) => (
                            <tr key={`${index}`}>
                              <td width='20%'>
                                <div align='center' className={s.stepItemStart}>
                                  <div className={s.ballPctMed}>
                                    <CircularProgressbar
                                      value={equip.pct} text={`${equip.pct}%`}
                                      background counterClockwise backgroundPadding={6}
                                      styles={buildStyles({
                                        backgroundColor: "#3a3a3a",
                                        pathColor: "#ffffff",
                                        textColor: "#fff",
                                        trailColor: '#708294',
                                      })} />
                                  </div>
                                  <div style={{ 'paddingLeft': '20px' }}>
                                    <br></br>
                                    {equip.equipmentName} / {equip.status}<br></br>
                                    <br></br>
                                  </div>
                                </div>
                              </td>
                              <td width='80%'>
                                <table width='100%' className={s.dot}>
                                  <tbody>
                                    <tr>
                                      {equip.steps.map((step, index) => (
                                        <td width='20%' key={`${index}`}>
                                          <div width='100%' align='center' className={s.stepItem}>
                                            <div>
                                              {
                                                equip.actualStep !== index + 1 ? <div>
                                                  <Tooltip placement="top"
                                                    isOpen={this.state.popoverDesc[step._id]}
                                                    target={`ball_${step._id}`}
                                                    toggle={() => this.toggleDesc(step)}>
                                                    {step.stepDescription}
                                                  </Tooltip>
                                                </div> : <div></div>
                                              }
                                              <Popover placement="bottom"
                                                isOpen={this.state.popoverHelp[step._id]}
                                                target={`ball_${step._id}`}
                                                toggle={() => this.toggleHelp(step)}>>
                                                <PopoverBody>
                                                  ETAPA {step._step}: <br></br>
                                                  <br></br>
                                                  Nesta etapa teremos:<br></br>
                                                  <br></br>
                                                  {this.state.currentPopoverText.map((stepDesc, index) => (
                                                    <table key={`${index}`}>
                                                      <tbody>
                                                        <tr>
                                                          <td><i className='fa fa-circle' /></td>
                                                          <td width='8px'></td>
                                                          <td>{stepDesc}</td>
                                                        </tr>
                                                      </tbody>
                                                    </table>
                                                  ))}
                                                  <br></br>
                                                </PopoverBody>
                                              </Popover>
                                              <br></br>
                                              <div id={`ball_${step._id}`} className={s.mousePointer} onClick={() => this.toggleHelp(step)}>
                                                {
                                                  step.pct === '100' ?
                                                    <div className={s.ballPctSm}><img alt=" " src={checkImg}></img></div>
                                                    :
                                                    <div className={s.ballPctSm}>
                                                      <CircularProgressbar
                                                        value={step.pct} text={`${step.pct}%`}
                                                        background counterClockwise backgroundPadding={6}
                                                        styles={buildStyles({
                                                          backgroundColor: step.pct === '0' ? "#9a9a9a" : step.pct === '100' ? "#4AAD51" : '#ffffff',
                                                          pathColor: step.pct === '0' ? "#81B3E6" : step.pct === '100' ? "#ffffff" : '#81B3E6',
                                                          textColor: "#000000",
                                                          trailColor: '#708294',
                                                        })} />
                                                    </div>
                                                }
                                              </div>
                                              <br></br>
                                              Conclusão:<br></br>
                                              {step.dtConclusion}<br></br>
                                              <br></br>
                                              {
                                                equip.actualStep === index + 1 ? <div>{step.stepDescription}</div> : <div></div>
                                              }
                                            </div>
                                          </div>
                                        </td>
                                      ))}
                                    </tr>
                                    <tr>
                                      <td colSpan='5'>
                                        <div style={{ marginTop: '-90px' }}>
                                          <TrackingChart typeChart={equip.actualStep} />
                                        </div>
                                      </td>
                                    </tr>
                                  </tbody>
                                </table>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  ))}
                  <br></br>
                </Widget>
              </div>
              :
              <div></div>
          }
        </div >
      )
  }
}

export default withRouter(ContractDetailPage);
