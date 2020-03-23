import React from "react";
import { Switch, Route } from "react-router";
import { TransitionGroup, CSSTransition } from "react-transition-group";
import Hammer from "rc-hammerjs";

import Header from "../Header";

import AssociadoQRCODE from "../../pages/associado/QRCODE/QRCODE";
import AssociadoLimites from "../../pages/associado/limites/Limites";
import AssociadoExtratos from "../../pages/associado/extratos/Extratos";
import AssociadoParcelamentos from "../../pages/associado/parcelamentos/Parcelamentos";
import AssociadoFaturas from "../../pages/associado/faturas/Faturas";
import AssociadoRede from "../../pages/associado/rede/Rede";
import AssociadoSolicitacoes from "../../pages/associado/solics/Solicitacoes";

import LojistaVenda from "../../pages/lojista/venda/venda";
import LojistaSolicitacoes from "../../pages/lojista/solics/Solicitacoes";
import LojistaAutorizacoes from "../../pages/lojista/autorizacoes/Autorizacoes";
import LojistaCancelamentos from "../../pages/lojista/cancelamentos/Cancelamentos";

import s from "./Layout.module.scss";

export default class LayoutComponent extends React.Component {

  render() {
    return (
      <div
        className={[
          s.root,
          "sidebar-" + this.props.sidebarPosition,
          "sidebar-" + this.props.sidebarVisibility
        ].join(" ")}
      >
        <div className={s.wrap}>
          <Header
            mainVars={this.props.mainVars}
            updateMainVars={this.props.updateMainVars}
          />
          <Hammer onSwipe={this.handleSwipe}>
            <main className={s.content}>
              <TransitionGroup>
                <CSSTransition key={this.props.location.pathname} classNames="fade" timeout={200}>
                  <Switch>
                    <Route path="/app/associado/QRCODE" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={AssociadoQRCODE} />
                    <Route path="/app/associado/limites" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={AssociadoLimites} />
                    <Route path="/app/associado/solics" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={AssociadoSolicitacoes} />
                    <Route path="/app/associado/extratos" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={AssociadoExtratos} />
                    <Route path="/app/associado/parcelamentos" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={AssociadoParcelamentos} />
                    <Route path="/app/associado/faturas" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={AssociadoFaturas} />
                    <Route path="/app/associado/rede" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={AssociadoRede} />
                    <Route path="/app/lojista/venda" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={LojistaVenda} />
                    <Route path="/app/lojista/solics" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={LojistaSolicitacoes} />
                    <Route path="/app/lojista/autorizacoes" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={LojistaAutorizacoes} />
                    <Route path="/app/lojista/cancelamentos" exact mainVars={this.props.mainVars} updateMainVars={this.props.updateMainVars} component={LojistaCancelamentos} />
                  </Switch>
                </CSSTransition>
              </TransitionGroup>
            </main>
          </Hammer>
        </div>
      </div>
    );
  }
}
