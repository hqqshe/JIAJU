require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'utils', 'domReady'], function ($, _, base, utils, _) {
        base.init();
        var part_item = '<tr>                                                                                                                                                                                                                                                                                                                                                                         \
                                    <td><input type="text" placeholder="部件名称" class="_input_area"></td>                                                                                                                                                                                                                     \
                                    <td><input type="text" placeholder="规格" class="_input_area"></td>                                                                                                                                                                                                                               \
                                    <td><input type="text" placeholder="数量" class="_input_area" onkeyup="this.value = this.value.replace(/\\D/g, \'\')" onafterpaste="this.value = this.value.replace(/\\D/g,\'\')"></td>     \
                                    <td><input type="text" placeholder="材料" class="_input_area"></td>                                                                                                                                                                                                                               \
                                    <td><a href="javascript:;" class="dele">×</a></td>                                                                                                                                                                                                                                                                         \
                                </tr>'
        var pid; //产品id      
        //加载数据
        window._load_items = function(_page_index) {
            $.when(base.loadHtmlData({ brandId: $("#brandId").val(), productName: $('.search_input').val(), pageIndex: _page_index }, "/ERPBrand/GetProductList")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //新建产品 导入产品数据 下载产品模板
        $("._btns").on("click", "a", function () {
            var index = $(this).index();
        });
        //编辑部件 添加
        $('.add_part').on("click", function () {
            $('.parts_con ._list_table tbody').append($(part_item).clone());
        });
        //编辑部件 删除
        $('.parts_con ._list_table').on("click", ".dele", function () {
            $(this).parent().parent().remove();
        });
        //编辑部件 提交
        $(".parts_con").on("click", ".sub_mit", function () {
            var parts_arr = [];
            var vali = false;
            $('.parts_con tbody tr').each(function (k, v) {
                var part = {};
                part.name = $(v).find('input').eq(0).val();
                if (!part.name) {
                    $.Message({ type: "alert", content: "请输入部件名称！" });
                    vali = true;
                    return false;
                }
                for (var i = 0; i < parts_arr.length; i++) {
                    if (parts_arr[i].name === part.name) {
                        $.Message({ type: "alert", content: "部件名称" + part.name + "已存在,请修改！" });
                        vali = true;
                        return false;
                    }
                }
                part.format = $(v).find('input').eq(1).val();
                if (!part.format) {
                    $.Message({ type: "alert", content: "请输入部件规格！" });
                    vali = true;
                    return false;
                }
                part.number = $(v).find('input').eq(2).val();
                if (!part.number) {
                    $.Message({ type: "alert", content: "请输入部件数量！" });
                    vali = true;
                    return false;
                }
                part.material = $(v).find('input').eq(3).val();
                if (!part.material) {
                    $.Message({ type: "alert", content: "请输入部件材料！" });
                    vali = true;
                    return false;
                }
                parts_arr.push(part);
            });
            if (vali) {
                return;
            }
            $.when(base.getCodeData({ 'productId': pid, 'partsList': utils.json2str(parts_arr) }, "/ERPBrand/savePartsInfo",'post')).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.parts_con').fadeOut();
            });
        });
        //订单详情
        $("._ajax_container").on("click", ".view", function () {
            var _id = $(this).attr("_id");
            var searchKey = $('.search_input').val();
            var brand = $('._opacity_select').val();
            window.location.href = "/ERPBrand/ProductDetail?id=" + _id + "#" + brand + "_" + searchKey;
        });
        //编辑部件面板
        $("._ajax_container").on("click", ".edit_part", function () {
            pid = $(this).attr("_id");
            $.when(base.getResponsData({ 'id': pid }, "/ERPBrand/getPartsInfo")).done(function (res) {
                fillPart(res.partsList);
                $('.parts_con').fadeIn();
            });
        });
        //填充部件
        function fillPart(data) {
            var con = $('.parts_con tbody');
            con.empty();
            if (data.length === 0) return;
            for (var i = 0; i < data.length; i++) {
                var item = $(part_item);
                item.children().eq(0).children().val(data[i].name);
                item.children().eq(1).children().val(data[i].format);
                item.children().eq(2).children().val(data[i].number);
                item.children().eq(3).children().val(data[i].material);
                con.append(item);
            }
        }
        //删除品牌
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            $.Message({
                type: "confirm", content: "你确定删除此产品吗?删除后该产品下面的所有内容将会清除！", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPBrand/DeleteProduct")).done(function () {
                        $.Message({ type: "alert", content: "删除成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});