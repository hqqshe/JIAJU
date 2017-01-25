require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'domReady'], function ($, _, base, _, _) {
        base.init();
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ departMentId: $(' ._opacity_select').val(), searchKey: $(' .search_input').val(), pageIndex: _page_index }, "/ERPemployer/GetEmployerList")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //打印
        $("._btns").on('click','a', function () {
            var index = $(this).index();
            if (index === 1) {
                var departmentId = parseInt($("._opacity_select").val());
                var searchKey = $(".search_input").val();
                window.open('/ERPemployer/PrintBarCodes?departmentId=' + departmentId + "&searchKey=" + searchKey)
            }
        });
        //删除员工
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            $.Message({
                type: "confirm", content: "确定要删除当前员工吗！", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPPartment/deleteDepartment")).done(function () {
                        $.Message({ type: "alert", content: "删除成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});
