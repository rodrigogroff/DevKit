
import React from 'react';
import {
    UncontrolledButtonDropdown,
    DropdownMenu,
    DropdownItem,
    DropdownToggle,
} from 'reactstrap';

export default class MultiSelectVortigo extends React.Component {

    render() {
        return (
            <div width='100%'>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                <UncontrolledButtonDropdown id="simple-big-select">
                                    <DropdownToggle caret color="default" size={this.props.size === undefined ? 'sm' : this.props.size} className="dropdown-toggle-split">
                                        <span className="mr-5"> {this.props.selectedOption}</span>
                                    </DropdownToggle>
                                    <DropdownMenu>
                                        {this.props.items.map((current, index) => (<DropdownItem key={`${current}${index}`} onClick={this.props.changeSelect}>{current}</DropdownItem>))}
                                    </DropdownMenu>
                                </UncontrolledButtonDropdown>
                            </td>
                            <td width='8px'></td>
                            <td>
                                {this.props.addSelectedItem !== undefined ? <i className='fa fa-plus' onClick={this.props.addSelectedItem} /> : <div></div>}
                            </td>
                        </tr>
                    </tbody>
                </table>
                {
                    this.props.selected_list !== undefined ?
                        this.props.selected_list.map((current, index) => (
                            <div key={`${current}${index + 2}`} width='100%'>
                                <i onClick={this.props.removeItem.bind(this, current)} className='la la-remove' /> <span>{current}</span></div>))
                        : <div></div>
                }
            </div>
        )
    }
}
