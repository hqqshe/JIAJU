require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'utils', 'domReady'], function ($, _, base, utils, _) {
        base.init();
        var pro_item = '<tr>                                                                                                                \
                                         <td>工序号</td>                                                                                    \
                                        <td><input type="text" placeholder="请输入工序名称" class="_input_area "></td>\
                                        <td><input type="text" placeholder="请输入工序价格" class="_input_area " onkeyup="this.value = this.value.replace(/[^0-9^.]/g, \'\')" onafterpaste="this.value = this.value.replace(/[^0-9]/g,\'\')"></td>\
                                        <td>请选择工段</td>                                                                           \
                                        <td class="_td_15 _over_text">                                                         \
                                            <a href="javascript:;" class="chose_gd">选择工段</a>     \
                                            <a href="javascript:;" class="change_pst">调整工序</a> \
                                            <a href="javascript:;" class="dele">×</a>                                 \
                                        </td>                                                                                                             \
                                    </tr>';
        var pid = window.location.href.split('=')[1];
        var _search = window.location.href;
        var _show_img = null;
        var parts_json;//所有部件
        var part_chose;//已选部件
        var pro_list;//工序列表
        //pro_list = [{ "id": "1", "name": "a", "price": 1, "stage_id": 1, "stage": "前工段" }, { "id": "2", "name": "b", "price": 2, "stage_id": 2, "stage": "前工段" }, { "id": "3", "name": "c", "price": 3, "stage_id": 3, "stage": "前工段" }, { "id": "4", "name": "d", "price": 4, "stage_id": 4, "stage": "前工段" }, { "id": "5", "name": "成品质检", "price": 2, "stage_id": 5, "stage": "前工段" }];

        //填充工序列表
        function fillPro() {
            var pro_tb = $('.pro_tb tbody');
            pro_tb.empty();
            for (var i = 0; i < pro_list.length; i++) {
                var item = $(pro_item);
                item.children().eq(0).text(i + 1);
                //item.children().eq(0).text(pro_list[i].id);
                item.children().eq(1).children().val(pro_list[i].name);
                item.children().eq(2).children().val(pro_list[i].price);
                item.children().eq(3).text(pro_list[i].stage).attr("_id", pro_list[i].stage_id).attr("_type", pro_list[i].type);
                if (pro_list[i].type != 0) {
                    item.find(".dele").remove();
                    item.find("input:eq(0)").attr("disabled", "disabled").css({"background-color":"#eee"});
                }
                pro_tb.append(item);
            }
        }
        //获取工序JSON数据
        function getProdureList()
        {
            _show_loading();
            $.ajax({
                type: "get",
                url: "/ERPBrand/getProcdureList",
                data: {id:pid},
                dataType: "json",
            success: function (response) {
                _hide_loading();
                if (response.code == 1) {
                    pro_list = response.data;
                    fillPro();
                } else {
                    $.Message({ type: "alert", content: response.message });
                }
            },
            error: function (e) {
                _hide_loading();
                $.Message({ type: "alert", content: "网络异常！" });
            }
        });
    }
    function renderData() {
        pro_list = [];
        $(".pro_tb tbody tr").each(function (i, o) {
            var item = {};
            $(o).find("td").each(function (k, v) {
                if (k == 0) {
                    item.id = $(v).text();
                } else if (k == 1) {
                    item.name = $(v).children().val();
                }
                else if (k == 2) {
                    item.price = $(v).children().val();
                }
                else if (k == 3) {
                    item.stage = $(v).text();
                    item.stage_id = $(v).attr("_id");
                    item.type = $(v).attr("_type")
                }

            });
            pro_list.push(item);
        });
    }
    $(document).ready(function () {
        getProdureList();
        //返回
        $('.back').on('click', function () {
            //window.location.href = "/ERPBrand/ProductList?" + _search.split("#")[1];
            window.location.href = "/ERPBrand/ProductList";
        });
        // 添加包号
        $('.add_bag').on("click", function () {
            var pro = {type:0};
            pro_list.unshift(pro);
            fillPro();
        });
        //工序输入保存到数组
        $(".pro_tb tbody").on("blur", "tr input", function () {
            renderData();
        });
        var _target_td = null;
        //选择工段面板
        $(".pro_tb").on("click", ".chose_gd", function () {
            _target_td = $(this).parent().prev();
            $('.gd_con').show();
        });
        //选择工段
        $("._select_gongduan").bind("click", function () {
            var checked = $(this).parent().find("input[type='radio']:checked");
            if (checked.length == 0) {
                $.Message({ type: "alert", content: "请选择工段" });
                return;
            }
            if (_target_td == null) {
                $.Message({ type: "alert", content: "请重新打开工段选择窗口！" });
            }
            var _val = checked.val();
            var _text = checked.parent().next().text().replace(/\s/g, "");
            _target_td.text(_text).attr("_id", _val);
            renderData();
            $(this).prev().click();
        });
        //调整顺序弹出层
        $(".pro_tb").on("click", ".change_pst", function () {
            $('.change_pos').show();
            old_pos = $(this).parents("tr").eq(0).children().eq(0).text();
            $('.old_pos').text(old_pos);
            renderData();
        });
        
        //调整顺序输入验证
        $(".pro_pos").on("blur", function () {
            var pos = $(this).val();
            var old_pos = parseInt($('.old_pos').text());
            if (pos == old_pos) {
                $.Message({ type: "alert", content: "调整序号相同！" });
                $(this).val("");
                return;
            }
            if (pos > pro_list.length) {
                $.Message({ type: "alert", content: "序号超出范围！" });
                $(this).val("");
                return;
            }
        });
        //确定调整
        $(".change_pos").on("click", ".change_ok", function () {
            var pos = $('.pro_pos').val();
            if (!pos) {
                $.Message({ type: "alert", content: "请输入调整序号！" });
                $(".pro_pos").val("");
                return;
            }
            pos = pos - 1;
            var old_pos = $('.old_pos').text() - 1;
            if (pos == old_pos) {
                $.Message({ type: "alert", content: "调整序号相同！" });
                $(".pro_pos").val("");
                return;
            }
            if (pos >= pro_list.length) {
                $.Message({ type: "alert", content: "序号超出范围！" });
                $(".pro_pos").val("");
                return;
            }
            var temp = pro_list.splice(old_pos, 1);
            pro_list.splice(pos, 0, temp[0]);
            fillPro();
            $('.change_pos').hide();
        });
        //编辑部件 删除
        $('.pro_tb').on("click", ".dele", function () {
            var index = $(this).parent().parent().index();
            pro_list.splice(index, 1);
            fillPro();
        });
    });
    //组织上传数据
    function GetSubmitData()
    {
        $(".pro_tb tbody tr").each(function (i, o) {
            pro_list[i].id = $(o).find("td:eq(0)").text().replace(/\s/g, "");
            pro_list[i].name = $(o).find("input:eq(0)").val().replace(/\s/g, "");
            pro_list[i].price = $(o).find("input:eq(1)").val().replace(/\s/g, "");
            pro_list[i].stage_id = $(o).find("td:eq(3)").attr("_id");
            pro_list[i].stage = $(o).find("td:eq(3)").text().replace(/\s/g, "");
            pro_list[i].type = $(o).find("td:eq(3)").attr("_type");
        });
        return pro_list;
    }
    //验证数据
    function ValidateSubmitData(data) {
        if (data.length == 0) {
            return false;
        }
        var _is_pass = true;
        for (var i = 0; i < data.length; i++)
        {
            if (data[i].name == "") {
                $.Message({ type: "alert", content: "第" + (i + 1) + "个工序未填写工序名称！" })
                _is_pass = false;
                break;
            }
            if (!/^(0|[1-9]\d{0,4})(\.\d{1,2})?$/.test(data[i].price)) {
                $.Message({ type: "alert", content: "第" + (i + 1) + "个工序价格格式错误！" })
                _is_pass = false;
                break;
            }
            if (pro_list[i].stage == "" ||typeof pro_list[i].stage_id == "undefined") {
                $.Message({ type: "alert", content: "第" + (i + 1) + "个工序还未选择工段！" })
                _is_pass = false;
                break;
            }
        }
        return _is_pass;
    }
    //提交数据
    $(".sub_mit").bind("click", function () {
        _show_loading();
        var _sub_data = GetSubmitData();
        if (!ValidateSubmitData(_sub_data)) {
            _hide_loading();
            return;
        }
        $.ajax({
            type: "post",
            url: "/ERPBrand/saveProcedure",
            data: {id:pid,processList:utils.json2str(pro_list)},
            dataType: "json",
        success: function (response) {
            _hide_loading();
            if (response.code == 1) {
                window.location.href = "/ERPBrand/ProductList";
            } else {
                $.Message({ type: "alert", content: response.message });
            }
        },
        error: function (e) {
            _hide_loading();
            $.Message({ type: "alert", content: "网络异常！" });
        }
    });
});
    });
});