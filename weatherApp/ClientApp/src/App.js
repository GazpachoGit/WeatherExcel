import React, { Component } from 'react';
import { Route, Redirect } from 'react-router';
import { Layout } from './components/Layout';
import  DataLoader  from './components/DataLoader';
import Weather from './components/Weather';

import './custom.css'


export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path="/" render={() => (
            <Redirect to="/weather"/>
        )}/>
        <Route exact path='/weather' component={Weather} />
        <Route path='/load-data' component={DataLoader} />
      </Layout>
    );
  }
}
