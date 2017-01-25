require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'domReady'], function ($, _, base, _, _) {
        base.init();
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({pageIndex: _page_index }, "/ERPPartment/GetPartmentList")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //添加部门
        $("._add_new_btn").live('click', function () {
            $("._add_partment_window").fadeIn();
        });
        //新建部门提交数据
        $("._sub_new_customer").live('click', function () {
            if ($("._add_partment_window").find("input").val().replace(/\s/g, "") == "") {
                $.Message({ type: "alert", content: "请输入生产线名称！" });
                return;
            }
            $.when(base.getCodeData({ department_name: $('._add_partment_window .partment').val(), 'describle': $('._add_partment_window .tip_txt').val() }, "/ERPPartment/saveDepartment", "post")).done(function () {
                _load_items(1);
                $.Message({ type: "alert", content: "添加成功！" });
                $("._add_partment_window").find("input,textarea").val("");
                $("._add_partment_window").fadeOut();
            });
        });
        //删除部门
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            $.Message({
                type: "confirm", content: "确定要删除当前生产线！", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPPartment/deleteDepartment")).done(function () {
                        $.Message({ type: "alert", content: "删除成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});
