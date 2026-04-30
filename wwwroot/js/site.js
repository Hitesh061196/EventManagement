// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(() => {
  const successNode = document.getElementById("swal-success");
  const errorNode = document.getElementById("swal-error");

  if (successNode?.dataset.message) {
    Swal.fire({
      icon: "success",
      title: "Success",
      text: successNode.dataset.message,
      timer: 2400,
      showConfirmButton: false
    });
  }

  if (errorNode?.dataset.message) {
    Swal.fire({
      icon: "error",
      title: "Attention",
      text: errorNode.dataset.message
    });
  }
})();
