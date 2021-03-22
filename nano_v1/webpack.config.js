const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');

module.exports = {
  mode: 'production',
  target: 'web',
  devtool: 'source-map',
  context: path.resolve(__dirname, './'),
  resolve: {
    alias: {
      '@app': path.resolve(__dirname, 'src/static/js')
    }
  },
  entry: {
    '/src/static/Login': './src/static/js/pages/Login/router.js',
    '/src/static/Dashboard': './src/static/js/pages/Dashboard/router.js',    
    '/src/static/Exit': './src/static/js/pages/Exit/router.js',    
    '/src/static/Limites': './src/static/js/pages/Limites/router.js',    
  },
  output: {
    globalObject: "this",
    path: path.resolve(__dirname, 'dist')
  },
  module: {
    rules: [{
      test: /\.js$/,
      exclude: /node_modules/,
      loader: 'babel-loader'
    },
    ]
  },
  plugins: [
    new CleanWebpackPlugin(),
  ]
}
