// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener('DOMContentLoaded', function () {
    const grid = document.querySelector('.app-grid');
    const btn = document.getElementById('sidebarToggle');

    if (grid && btn) {
        btn.addEventListener('click', () => {
            const collapsed = grid.classList.toggle('sidebar-collapsed');
            btn.setAttribute('aria-pressed', collapsed ? 'true' : 'false');

            // If you use DataTables in .app-main, nudge it after layout change
            if (window.jQuery && jQuery.fn.dataTable) {
                jQuery('.dataTable').each(function () {
                    const api = jQuery(this).DataTable();
                    api.columns.adjust().responsive.recalc();
                });
            }
        });
    }
});