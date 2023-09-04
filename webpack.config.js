const path = require('path');

module.exports = {
  watch:false,
  entry: './wwwroot/Assets/Scripts/index.js',
  output: {
    filename: 'scripts.min.js',
    path: path.resolve(__dirname, 'wwwroot/Assets/Scripts/minified'),
  },
  resolve: {
    modules: [
      path.join(__dirname, "wwwroot/Assets/Vendors"),
      "node_modules"
    ]
  },
  externals: {
    // require("jquery") is external and available
    //  on the global var jQuery
    "jquery": "jquery"
  }
};