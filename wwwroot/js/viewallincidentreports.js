$(document).ready(function () {
    $('#Dttbl_incidentReportsView').DataTable({
        lengthMenu: [[5, 10, 25, 50, 100, 1000], [5, 10, 25, 50, 100, 1000]],
        paging: true,
        pagingType: "full_numbers",
        ordering: true,
        scrollCollapse: true,
        fixedHeader: true,
        order: [[0, "desc"]],
        info: false,
        scrollX: true,
        searching: true,
        autoWidth: false,
        ajax: {
            url: '/ViewReports?handler=ViewAllReports',
            dataSrc: '',
            error: function (xhr, error, thrown) {
                console.error('Error loading data:', error);
            }
        },
        columns: [
            { data: 'id', visible: false },
            { data: 'report_number', width: '20%', className: "text-center" },
            { data: 'nosmpr_rpt_dtm', width: '20%', className: "text-center" },
            { data: 'nosmpr_name_of_witness', width: '20%', className: "text-center" },
            {
                data: null,
                width: '10%',
                className: "text-center",
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-primary send-email" data-id="${row.id}">
                            <i class="fas fa-envelope"></i>
                        </button>
                    `;
                }
            }
        ]
    });

    $('div.dataTables_filter').addClass('mb-2');

    $('#Dttbl_incidentReportsView').on('click', '.send-email', function () {
        const rowId = $(this).data('id');
        sendEmail(rowId);
    });
});

function sendEmail(rowId) {
    $.ajax({
        url: '/ViewReports?handler=SendEmail',
        data: { id: parseInt(rowId) },
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        type: 'POST',

        success: function () {
            alert('Email sent successfully!');
        },
        error: function (xhr, status, error) {
            alert('Failed to send email. Error: ' + error);
        }
    });
}
