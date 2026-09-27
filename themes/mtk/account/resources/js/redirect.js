(function () {
  var basePath = "/realms/mtk/account";
  var current = location.pathname.replace(/\/$/, "");
  if (current === basePath) {
    location.replace(basePath + "/account-security/signing-in");
  }
})();
