$(function () {
    // Initialize DataTable on page load
    initializeDataTable();
});

// Function to initialize DataTable
window.initializeDataTable = function ()
{
    // Destroy existing instance first (to avoid duplication)
    if ($.fn.DataTable.isDataTable('#index-table'))
    {
        $('#index-table').DataTable().destroy();
    }

    // Re-initialize
    $('#index-table').DataTable({
        pageLength: 10
    });
};