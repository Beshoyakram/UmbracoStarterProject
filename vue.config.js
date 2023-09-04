const path = require("path");

module.exports = {
    outputDir: path.resolve(__dirname, "./wwwroot/dist"),
    assetsDir: "../",
    filenameHashing:false ,
    productionSourceMap : false ,
  chainWebpack: (config) => {
    config
      .entry("main-ltr")
      .add("./src/assets/sass/main-ltr.scss")
      .end();
    config
      .entry("main-rtl")
      .add("./src/assets/sass/main-rtl.scss")
      .end();
  },
  css: {
    extract: {
      filename: "[name].css", // to have a name related to a theme
      chunkFilename: "css/[name].css",
    },
    modules: false,
    sourceMap: false,
  },
};
