function confirmBorrowRequest(id) {
    $.ajax({
        url: '/BorrowRequests/ConfirmBorrowRequest/' + id, 
        type: 'GET',
        success: function (data) {
            $('#borrowRequestList').html(data);
            alert("Позика підтверджена");
        },
        error: function (xhr, status, error) {
            console.error("Error occurred: " + error);
            alert("Щось пішло не так. Спробуйте ще раз.");
        }
    });
}
