mmmf.factories.metaTags = function (apiRoot, ajaxFx) {

	var thisService = this;
	thisService.apiRoot = apiRoot;
	thisService.readType = _readType;
	thisService.update = _update;
	thisService.add = _add;
	thisService.ajax = ajaxFx;


	function _readType(ownerId, ownerType, onSuccess, onError) {
		var link = apiRoot + "/" + ownerId + "/" + ownerType;
		var settings = {
			cache: false,
			contentType: "application/x-www-form-urlencoded; charset=UTF-8",
			type: 'get',
			dataType: "json",
			success: onSuccess,
			error: onError
		}
		thisService.ajax(link, settings);
	}

	function _update(pageMetaTagId, data, onSuccess, onError) {
		var link = apiRoot + "/" + pageMetaTagId;
		var settings = {
			cache: false,
			contentType: "application/x-www-form-urlencoded; charset=UTF-8",
			type: 'put',
			data: data,
			dataType: "json",
			success: onSuccess,
			error: onError
		}
		thisService.ajax(link, settings);
	}

	function _add(data, metaTagId, onSuccess, onError) {

		var settings = {
			cache: false,
			contentType: "application/x-www-form-urlencoded; charset=UTF-8",
			type: 'post',
			data: data,
			dataType: "json",
			success: function (responseData) {

				onSuccess(responseData, metaTagId);

			},
			error: onError

		};


		thisService.ajax(apiRoot, settings);
	}
}

mmmf.services.metaTags = new mmmf.factories.metaTags('/api/metatags', $.ajax);