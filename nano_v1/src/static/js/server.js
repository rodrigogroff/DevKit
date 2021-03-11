
const compression = require('compression');
const express = require("express");
const path = require("path");
const app = express();
const shouldCompress = (req, res) => {
  if (req.headers['x-no-compression']) return false;
  return compression.filter(req, res);
};
app.use(compression({ filter: shouldCompress, threshold: 0 }));
app.use("/src", express.static(path.resolve(__dirname, "src")));
app.get("/", (req, res) => { res.sendFile(path.resolve(__dirname, "./index_dashboard.html")); });
app.get("/login", (req, res) => { res.sendFile(path.resolve(__dirname, "./index_login.html")); });
app.get("/exit", (req, res) => { res.sendFile(path.resolve(__dirname, "./index_exit.html")); });
app.listen(80, () => console.log("Server running..."));
