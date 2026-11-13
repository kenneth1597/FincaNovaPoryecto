(function () {
    var boton = document.getElementById('fnMenuButton');
    var fondo = document.getElementById('fnBackdrop');
    if (!boton || !fondo) return;

    function abrir(abierto) {
        document.body.classList.toggle('fn-sidebar-open', abierto);
        boton.setAttribute('aria-expanded', abierto ? 'true' : 'false');
    }

    boton.addEventListener('click', function () {
        abrir(!document.body.classList.contains('fn-sidebar-open'));
    });

    fondo.addEventListener('click', function () {
        abrir(false);
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') abrir(false);
    });
})();
