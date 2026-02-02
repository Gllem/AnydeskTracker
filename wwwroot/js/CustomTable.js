let editingTable = false;

function CustomTable(tableSelector, bulkUpdateUrl, updateCallback, drag){
    const table = $(tableSelector);

    table.on('click', '.editable', function () {
        if (editingTable) return;

        const cell = $(this);
        const oldValue = cell.text().trim();

        let input = $(`<input type="text" class="form-control form-control-sm flex-wrap m-0">`);
        input.val(oldValue);

        cell.data('original', oldValue);
        cell.empty().append(input);
        input.focus();

        editingTable = cell;
    });

    table.on('change', '.row-check', function () {
        const checkBox = $(this);
        const newVal = checkBox.is(':checked') ? '1' : '0';
        const oldVal = String(checkBox.data('original'));

        if (newVal !== oldVal) {
            checkBox.closest('tr').addClass('table-warning');
            checkBox.data('changed', true);
        } else {
            checkBox.data('changed', false);
            updateRowWarning(checkBox.closest('tr'));
        }
    });

    table.on('blur', 'input, select', function () {
        finishEdit($(this).closest('.editable'), true);
    });

    table.on('keydown', 'input, select', function (e) {
        if (e.key === 'Enter') {
            finishEdit($(this).closest('.editable'), true);
        }
        if (e.key === 'Escape') {
            finishEdit($(this).closest('.editable'), false);
        }
    });

    if(drag){
        $(`${tableSelector} tbody`).sortable({
            handle: '.drag-handle',
            axis: 'y',
            helper: 'clone',
            placeholder: 'sortable-placeholder',

            start: function () {
                if (this.editingTable) {
                    return false;
                }
            },

            update: function () {
                updateOrder(tableSelector);
            }
        });
    }

    $('#saveBtn').on('click', function () {
        const changes = [];

        $(`${tableSelector} tbody tr.table-warning`).each(function () {
            const row = $(this);
            const item = {
                id: row.data('id'),
                sortOrder: row.data('order')
            };

            row.find('.editable').each(function () {
                const cell = $(this);
                item[cell.data('field')] = cell.text().trim();
            });

            row.find('.row-check').each(function () {
                const checkbox = $(this);
                item[checkbox.data('field')] = checkbox.is(':checked');
            });

            changes.push(item);
        });

        if (!changes.length) {
            alert('Нет изменений');
            return;
        }

        $.ajax({
            url: bulkUpdateUrl,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(changes),
            success: () => {
                updateCallback();
            },
            error: () => alert('Ошибка')
        });
    });
    
    function updateRowWarning(row) {
        const hasEditableChanges = row.find('.editable').toArray().some(td => $(td).data('changed') === true);
        const hasCheckboxChanges = row.find('.row-check').toArray().some(cb => $(cb).data('changed') === true);

        if (hasEditableChanges || hasCheckboxChanges)
            row.addClass('table-warning');
        else
            row.removeClass('table-warning');
    }

    function finishEdit(cell, save) {
        const input = cell.find('input, select');
        const newValue = input.val();
        const oldValue = cell.data('original');

        cell.empty().text(save ? newValue : oldValue);

        if (save && newValue !== oldValue) {
            cell.data('changed', true);
        }
        else{
            cell.data('changed', false);
        }

        updateRowWarning(cell.closest('tr'));
        editingTable = false;
    }

    function updateOrder(tableSelector) {
        $(`${tableSelector} tbody tr`).each(function (index) {
            $(this)
                .attr('data-order', index + 1)
                .addClass('table-warning')
                .data('orderChanged', true);
        });
    }
}
