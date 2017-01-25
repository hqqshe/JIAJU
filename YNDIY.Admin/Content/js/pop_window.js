/*
//弹窗类型
type:alert、confirm、error、success、info、wait [default:alert]

//标题
title:

//内容
content:

//确定按钮文本
okText:

//取消按钮文本
cancelText:

//确定回调函数
okFun:

//取消回调函数
cancelFun:

//显示关闭按钮
showClose:true、false [default:true]

//自动关闭
autoClose:true、false [default:false]

//自动关闭时间(单位毫秒)
autoCloseTime:1000 [default:1000]

//关闭回调函数
closeFun:

//是否可拖拽
draggable:

//esc按键关闭是否开启
escable:

//自动关闭回调函数
autoCloseCallBack：
*/
(function($){
	$.Message = function(_params){
		var _defaults = {
			type:"",
			title: "提示！",
			content:"",
			okText:"确定",
			cancelText:"取消",
			okFun:null,
			cancelFun:null,
			showClose:true,
			autoClose:false,
			autoCloseTime:2000,
			closeFun:null,
			draggable:true,
			escable:true,
			autoCloseCallBack:null
		}
		
		var _options = {};
		_options = $.extend({},_defaults,_params);
		
	    //弹窗遮罩层
		var _meg_mask = '<div class="_d_message_mask"></div>';
		//Message弹窗HTML 
		var _msgHtml = 	'<div class="_d_message_outLine _d_message_window_shoadow">'+
							'<div class="_d_message_bg_opacity"></div>'+
							'<div class="_d_mesage_panel _d_mesage_panel_bg _clear_panel">'+
								'<div class="_left_box _d_message_icon"></div>'+
								'<div class="_right_box _d_message_content"></div>'+
								'<div class="_clear_float"></div>'+
								'<div class="_d_message_close">X</div>'+
							'</div>'+
						'</div>';
		//Alert弹窗HTML
		var _alertHtml = '<div class="_d_window_outline _d_message_window_shoadow _clear_panel">'+
								'<div class="_d_widnow_title">'+
									'<span></span>'+
									'<div class="_d_window_close">X</div>'+
								'</div>'+
								'<div class="_d_window_content"></div>'+
								'<div class="_d_window_buttons _clear_panel">'+
									'<a href="javascript:void(0);" class="_d_window_ok">确定</a>'+
									'<div class="_clear_float"></div>'+
								'</div>'+
							'</div>';
		//Confirm弹窗HTML
		var _confirmHtml = '<div class="_d_window_outline _d_message_window_shoadow _clear_panel">' +
								'<div class="_d_widnow_title">'+
									'<span></span>' +
									'<div class="_d_window_close">X</div>'+
								'</div>'+
								'<div class="_d_window_content"></div>'+
								'<div class="_d_window_buttons _clear_panel">'+
									'<a href="javascript:void(0);" class="_d_confirm_cancel _right_box">取消</a>' +
									'<a href="javascript:void(0);" class="_d_confirm_ok _right_box">确定</a>' +
									'<div class="_clear_float"></div>'+
								'</div>'+
							'</div>';
							
		var _html;
		
		//关闭弹窗
		function closeWindow(_obj) {
		    $("._d_message_mask").remove();
			$(_obj).remove();
		}
		
		//所有参数配置
		{
			//判断窗口类型
			switch(_options.type){
				case "alert":
					_html = $(_alertHtml);
					break;
				case "confirm":
					_html = $(_confirmHtml);
					break;
				case "error":
				case "success":
				case "wait":
				case "info":
					_html = $(_msgHtml);
					break;
				default:
					_html = $(_alertHtml);
					break;			
			}
			//title 参数赋值
			if(_options.title != ""){
				switch(_options.type){
					case "alert":
					case "confirm":
						$(_html).find("._d_widnow_title span").text(_options.title);
						break;
					default:
						break;			
				}
			}
			//content 参数赋值
			if(_options.content != ""){
				switch(_options.type){
					case "alert":
					case "confirm":
					    $(_html).find("._d_window_content").append($("<div>" + _options.content + "</div>"));
						break;
					case "error":
					case "success":
					case "wait":
					case "info":
					    $(_html).find("._d_message_content").append($("<div>" + _options.content + "</div>"));
						break;
					default:
						break;			
				}
			}
			//okText 赋值
			if(_options.okText != ""){
				switch(_options.type){
					case "alert":
						$(_html).find("._d_window_ok").text(_options.okText);				
						break;
					case "confirm":
						$(_html).find("._d_confirm_ok").text(_options.okText);
						break;
					default:
						break;			
				}
			}
			//cancelText 赋值
			if(_options.cancelText != ""){
				switch(_options.type){
					case "confirm":
						$(_html).find("._d_confirm_cancel").text(_options.cancelText);
						break;
					default:
						break;			
				}
			}
			//okFun cancelFun closeFun 回调函数		
			switch(_options.type){
				case "alert":
					$(_html).find("._d_window_ok").click(function(){
						if((typeof _options.okFun) == "function"){
							_options.okFun();
						}
						closeWindow(_html);
					});
					$(_html).find("._d_window_close").click(function(){
						if((typeof _options.closeFun) == "function"){
							_options.closeFun();
						}
						closeWindow(_html);
					});
					break;
				case "confirm":
					$(_html).find("._d_confirm_ok").click(function(){
						if((typeof _options.okFun) == "function"){
							_options.okFun();
						}
						closeWindow(_html);
					});
					$(_html).find("._d_confirm_cancel").click(function(){
						if((typeof _options.cancelFun) == "function"){
							_options.cancelFun();
						}
						closeWindow(_html);
					});
					$(_html).find("._d_window_close").click(function(){
						if((typeof _options.closeFun) == "function"){
							_options.closeFun();
						}
						closeWindow(_html);
					});
					break;
				case "error":
				case "success":
				case "wait":
				case "info":
					$(_html).find("._d_message_close").click(function(){
						if((typeof _options.closeFun) == "function"){
							_options.closeFun();
						}
						closeWindow(_html);
					})
					break;
				default:
					break;			
			}
			//如果不显示关闭按钮，就把关闭按钮隐藏
			if(!_options.showClose){
				switch(_options.type){
					case "error":
					case "success":
					case "wait":
					case "info":
						$(_html).find("._d_message_close").hide();
						break;
					default:
						break;			
				}
			}
			
		}
		
		//设置窗口宽度
		function setWindowWidth(_obj){
			if(!/^(alert|confirm)$/.test(_options.type)){
				$("._d_message_outLine").width(($("._d_message_content").width()+62)+"px");
			}
		}
		//设置窗口居中
		function windowToCenter(_obj){
			setWindowWidth(_obj);
			var _w_width = $(window).width(),
				_w_height = $(window).height(),
				_this_width = $(_obj).width(),
				_this_height = $(_obj).height();
			$(_obj).css({top:Math.floor((_w_height - _this_height)/2)+"px",left:Math.floor((_w_width - _this_width)/2)+"px"});
		}
		//拖拽事件
		function draggEvent(bar,panel){
			var mouseDown = false,
				x_margin = 0,
				y_margin = 0,
				movePanel = $(panel),
			    _dom_scroll_left = 0,
			    _dom_scroll_top = 0;
			$(bar).mousedown(function(e){
				$("body").addClass("_no_select");
				mouseDown = true;
				x_margin = e.pageX - movePanel.offset().left;
				y_margin = e.pageY - movePanel.offset().top;
			}).mouseup(function(){
				$("body").removeClass("_no_select");
				mouseDown = false;
			});
		    $("body").mousemove(function (e) {
				if(mouseDown){
		            _dom_scroll_left = $(document).scrollLeft();
		            _dom_scroll_top = $(document).scrollTop();
				    movePanel.css({ top: (e.pageY - y_margin - _dom_scroll_top) + "px", left: (e.pageX - x_margin - _dom_scroll_left) + "px" });
				}
				return;
			});
		}
		//设置窗口拖拽
		function dragWindow(_obj){
			if(_options.draggable){
				var _dragBar,_dragPanel = _obj;
				if(/^(alert|confirm)$/.test(_options.type)){
					_dragBar = $(_obj).find("._d_widnow_title");
					//_dragPanel = $(_obj).find("._d_window_outline");
				}else{
					_dragBar = $(_obj).find("._d_mesage_panel");
					//_dragPanel = $(_obj).find("._d_message_outLine");
				}
				$(_dragBar).css({cursor:"move"});
				draggEvent(_dragBar,_dragPanel);
			}
		}
		//绑定ESC按键退出
		function ESC(_obj){
			$("body").keyup(function(e){
				if(e.keyCode == 27){
					if(/^(alert|confirm)$/.test(_options.type)){
						$(_obj).find("._d_window_close").click();
					}else{
						$(_obj).find("._d_message_close").click();
					}
				}
			});
		}
		//自动关闭
		function autoCloseWindow(_obj){
			setTimeout(function(){
					//自动关闭回调函数
					if(typeof(_options.autoCloseCallBack) == "function"){
						_options.autoCloseCallBack();
					}
					_obj.fadeOut(500, function () { $("._d_message_mask").remove(); _obj.remove(); });
				},_options.autoCloseTime);
		}
		//渲染控件
		function render() {
		    $("body").append($(_meg_mask)).append(_html);
			windowToCenter(_html);
			dragWindow(_html);
			
			//是否启用ESC退出
			if(_options.escable){
				ESC(_html);
			}
			//是否启用自己退出
			if(_options.autoClose){
				autoCloseWindow(_html)
			}
		}
		
		render();
	}
})(jQuery);