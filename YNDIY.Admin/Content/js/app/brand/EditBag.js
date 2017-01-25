require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'utils', 'domReady'], function ($, _, base, utils, _) {
        base.init();
        var bag_item = ' <tr>                                                                                                                 \
                                        <td class="_td_15 _over_text"></td>                                                \
                                        <td></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                        <td class="_td_15 _over_text">                                                           \
                                            <a href="javascript:;" class="chosen_part">选择部件</a> \
                                             <a href="javascript:;" class="gy">关联产品</a>                     \
                                            <a href="javascript:;" class="dele text_info">×</a>               \
                                        </td>                                                                                                               \
                                    </tr>';
        var part_item = '  <tr>                                                                                                               \
                                        <td></td>                                                                                                      \
                                        <td class="_td_15 _over_text">                                                           \
                                            <a href="javascript:;" class="chose">选择</a>                         \
                                        </td>                                                                                                               \
                                    </tr>';
        var right_part = ' <tr>                                                                                                                \
                                        <td></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                        <td class="_td_15 _over_text">                                                           \
                                            <a href="javascript:;" class="up">上移</a>                               \
                                            <a href="javascript:;" class="down ">下移</a>                        \
                                            <a href="javascript:;" class="dele">×</a>                                   \
                                        </td>                                                                                                               \
                                    </tr>';
        var pro_item = ' <tr>                                                                                                                 \
                                        <td><input type="radio" name="pro"></td>                                 \
                                        <td></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                        <td class="_td_20 _over_text"></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                    </tr>';

        var pid = window.location.search.split("=")[1];
        var parts_json;//所有部件
        var part_Selected;//所有部件 已选
        var part_chose;//已选部件
        var all_data = {};//所有数据
        var current_index = 0;//当前数组索引
        getPart();
        initAllData();
        getAllData();
        //初始所有数据
        function initAllData() {
            all_data = {};
            all_data.id = pid;
            all_data.packageList = [];
        }
        //获取所有部件
        function getPart() {
            $.when(base.getResponsData({ 'id': pid }, "/ERPBrand/getPartsInfo")).done(function (res) {
                parts_json = res.partsList;
            });
        }
        //获取所有数据
        function getAllData() {
            $.when(base.getData({ 'id': pid }, "/ERPBrand/GetPackageInfo")).done(function (res) {
                all_data.packageList = res;
            });
        }
        // 添加包号
        $('.add_bag').on("click", function () {
            var tb = $('.bag_tab tbody');
            var item = $(bag_item);
            $('.bag_tab tbody').append(item);
            var count = tb.children().length;
            tb.children().each(function (k, v) {
                $(v).children().eq(0).text(count + "-" + (k + 1));
                var bag = all_data.packageList[k];
                if (bag) {
                    bag.packageNumber = count + "-" + (k + 1)
                } else {
                    bag = {};
                    bag.packageNumber = count + "-" + (k + 1)
                    bag.number = 0;
                    bag.lockNumber = 0;
                    bag.relation_id = 0;
                    all_data.packageList.push(bag);
                }
               
            });
        });
        // 删除包号
        $('.bag_tab').on("click", ".dele", function () {
            var that = $(this)
            $.Message({
                type: "confirm", content: "确定删除 " + that.parent().parent().children().first().text() + " 包吗,删除后此包下面的部件也将会删除！", okFun: function () {
                    all_data.packageList.splice(that.parent().parent().index(), 1);
                    that.parent().parent().remove();
                    var tb = $('.bag_tab tbody');
                    var count = tb.children().length;
                    tb.children().each(function (k, v) {
                        $(v).children().eq(0).text(count + "-" + (k + 1))
                        var bag = all_data.packageList[k];
                        bag.packageNumber = count + "-" + (k + 1)
                    });
                }
            });
        });
        //选择部件面板 打开
        $(".bag_tab").on("click", ".chosen_part", function () {
            current_index = $(this).parents('tr').eq(0).index();
            part_chose = all_data.packageList[current_index].partsList;
            fillPart();
            $('.parts_con').fadeIn();
        });
        //填充部件面板
        function fillPart() {
            var parts_tb = $('.parts_con .parts_tb tbody');
            var right_parts_tb = $('.parts_con .right_parts_tb tbody');
            parts_tb.empty();
            right_parts_tb.empty();
            if (part_chose === undefined || part_chose === null) {
                part_chose = [];
            }
            for (var j = 0; j < part_chose.length; j++) {
                var item = $(right_part);
                item.children().eq(0).text(j + 1);
                item.children().eq(1).text(part_chose[j].name);
                if (1 === part_chose.length) {
                    item.find(".up").remove();
                    item.find(".down").remove();
                } else if (j === 0) {
                    item.find(".up").remove();
                } else if (j === part_chose.length - 1) {
                    item.find(".down").remove();
                }
                right_parts_tb.append(item);
            }
            for (var i = 0; i < parts_json.length; i++) {
                var bool = false;
                outloop:
                    for (var k = 0; k < all_data.packageList.length; k++) {
                        if (!all_data.packageList[k].partsList) continue;
                        for (var j = 0; j < all_data.packageList[k].partsList.length; j++) {
                            if (parts_json[i].name === all_data.packageList[k].partsList[j].name) {
                                bool = true;
                                break outloop;
                            }
                        }
                    }
                if (bool) continue;
                var item = $(part_item);
                item.children().first().text(parts_json[i].name);
                parts_tb.append(item);
            }
        }
        //选择部件事件
        $('.parts_con .parts_tb tbody').on("click", ".chose", function () {
            var name = $(this).parent().prev().text();
            for (var i = 0; i < parts_json.length; i++) {
                if (parts_json[i].name === name) {
                    var part = parts_json[i];
                    break;
                }
            }
            part_chose.push(part);
            fillPart();
        });
        //删除部件事件
        $('.parts_con .right_parts_tb tbody').on("click", ".dele", function () {
            var tr = $(this).parent().parent();
            var name = tr.children().eq(1).text();
            for (var j = 0; j < part_chose.length; j++) {
                if (name === part_chose[j].name) {
                    part_chose.splice(j, 1);
                    break;
                }
            }
            fillPart();
        });
        //部件上移事件
        $('.parts_con .right_parts_tb tbody').on("click", ".up", function () {
            var tr = $(this).parent().parent();
            var name = tr.children().eq(1).text();
            for (var j = 0; j < part_chose.length; j++) {
                if (j === 0) continue;
                if (name === part_chose[j].name) {
                    var part = part_chose.splice(j, 1);
                    part_chose.splice(j - 1, 0, part[0]);
                    break;
                }
            }
            fillPart();
        });
        //部件下移事件
        $('.parts_con .right_parts_tb tbody').on("click", ".down ", function () {
            var tr = $(this).parent().parent();
            var name = tr.children().eq(1).text();
            for (var j = 0; j < part_chose.length; j++) {
                if (j === part_chose.length - 1) continue;
                if (name === part_chose[j].name) {
                    var part = part_chose.splice(j, 1);
                    part_chose.splice(j + 1, 0, part[0]);
                    break;
                }
            }
            fillPart();
        });
        //提交部件
        $('.parts_con').on("click", ".sub_mit ", function () {
            all_data.packageList[current_index].partsList = part_chose;
            var text = "";
            for (var i = 0; i < part_chose.length; i++) {
                text += part_chose[i].name + ",";
            }
            $('.bag_tab tbody tr').eq(current_index).children().eq(1).text("").text(text.substring(0, text.length - 1));
            $('.parts_con').fadeOut();
            if (text !== '') {
                all_data.packageList[current_index].relation_id = 0
                $('.bag_tab tbody tr').eq(current_index).children().eq(2).text("无")
            }
        });
        //关联产品面板
        $(".bag_tab").on("click", ".gy", function () {
            current_index = $(this).parents('tr').eq(0).index();
            if (all_data.packageList[current_index].partsList && all_data.packageList[current_index].partsList.length !== 0) {
                $.Message({ type: "alert", content: "此包包含部件,无法再关联产品,请先取消包含部件,保存之后再来关联产品！" });
                return;
            }
            $('.pro_gy_con').fadeIn();
        });
        //提交关联
        $('.pro_gy_con').on("click", ".sub_mit ", function () {
            var tr = $('.pro_tab tbody input[type=radio]:checked').parent()
            var rid = tr.attr("_id");
            if (rid) {
                if (rid === all_data.id) {
                    $.Message({ type: "alert", content: "不能关联当前产品！" });
                    return;
                }
                var text = tr.next().next().text();
                $('.bag_tab tbody tr').eq(current_index).children().eq(2).text(text);
            } else {
                rid = 0;
            }
            all_data.packageList[current_index].relation_id = rid;
            $('.pro_gy_con').fadeOut();
        });
        //查询关联产品
        $(".pro_gy_con .search_btn").on("click", function () {
            var _val = $('.pro_gy_con .search_input').val();
            if (!_val) {
                $.Message({ type: "alert", content: "请输入产品型号！" });
                return;
            }
            $.when(base.getData({ searchKey: _val }, "/ERPBrand/GetRelationProducts")).done(function (res) {
                fillPro(res);
            });
        });
        //填充关联产品
        function fillPro(data) {
            var con = $('.pro_gy_con .pro_tab tbody');
            con.empty();
            if (data.length === 0) return;
            for (var i = 0; i < data.length; i++) {
                var item = $(pro_item);
                item.children().eq(0).attr("_id", data[i].id);
                item.children().eq(1).text(data[i].produce_name);
                item.children().eq(2).text(data[i].model_name);
                item.children().eq(3).text(data[i].format).attr('title', data[i].format);
                item.children().eq(4).text(data[i].color);
                item.children().eq(5).text(data[i].unit);
                item.children().eq(6).text(data[i].standard_price);
                con.append(item);
            }
        }
        //提交所有数据
        $(".sub_mit_all").on("click", function () {
            for (var i = 0; i < all_data.packageList.length; i++) {
                if (!all_data.packageList[i].partsList && all_data.packageList[i].relation_id == 0) {
                    $.Message({ type: "alert", content: all_data.packageList[i].packageNumber + " 包没有包含部件,并且无关联产品,请先包含或者关联" });
                    return;
                }
            }
            $.when(base.getCodeData({ id: all_data.id, packageList: utils.json2str(all_data.packageList) }, "/ERPBrand/saveBag", "post")).done(function () {
                location.href='/ERPBrand/ProductList';
            });
        });
    });
});