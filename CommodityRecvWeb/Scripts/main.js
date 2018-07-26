
function AjaxPost(url, data, bAsync, cbSuccess, cbError, cbBeforeSend, cbCompleteSend) {
    $.ajax({
        type: "POST",
        data: data,
        cache: false,
        url: url,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: !!bAsync,
        success: cbSuccess,
        error: cbError || function (xhr, status, errorThrown) {
                console.log("Error: " + errorThrown);
                console.log("Status: " + status);
                console.dir(xhr);
        },
        beforeSend: cbBeforeSend,
        complete: cbCompleteSend
    });
}

function AjaxPostForm(url, idForm, bAsync, cbSuccess, cbError, cbBeforeSend, cbCompleteSend) {
    var formData = new FormData($(idForm)[0]);
    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        async: !!bAsync,
        cache: false,
        contentType: false,
        processData: false,
        success: cbSuccess,
        error: cbError || function (xhr, status, errorThrown) {
            console.log("Error: " + errorThrown);
            console.log("Status: " + status);
            console.dir(xhr);
        },
        beforeSend: cbBeforeSend,
        complete: cbCompleteSend
    });
}


function reload() {
    window.location.reload(true);
}

function ShowModalDlg(id, bShow) {
    if (bShow) {
        $(id).modal('show');
    } else {
        $(id).modal('hide');
    }
}

function TTabHelper($tabLec) {
    var self = this;
    self.$tab = $tabLec;

    self.AddRow = function (entry) {
        var rowNode = self.$tab
            .row.add(entry)
            .draw()
            .node();
        return rowNode;
    };
    self.GetAllData = function () {
        var dt = self.$tab.rows().data().toArray();
        return dt;
    }
    self.GetDataByCell = function (cell) {
        var dt = self.$tab.row($(cell).parents('tr')).data();
        return dt;
    }
    self.RemoveRow = function (cell) {
        self.$tab
            .row($(cell).parents('tr'))
            .remove()
            .draw();
    }
}



