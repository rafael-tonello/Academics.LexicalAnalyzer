/* The  mission  of  this service is to preload a list of objects from the server.
 * the  goal  is to force the browser and SJL to cache all resources and improve
 * system performance by reducing the charging times */
AppCacher = function(_observer_callback_, _startupDelay_){
	_startupDelay_ = _startupDelay_ || 1;
	_observer_callback_ = _observer_callback_ || function(){};
	
	this.private.observers = [_observer_callback_];
	//chargins is make in order of declaration in the vector bellow
	this.private.objectsList = [
		//modules
		"Activities/Main.html",
		"Activities/ImageParameters.html",
		"Shared/Components/MessageDialog.html",
		"Shared/Components/CircularButton.html",
		"Shared/Components/ColorButton.html",
		"Shared/Components/HorizontalScrollAnimation.html",
		"Shared/Components/NumericConfigItem.html",
		"Shared/Components/RangeConfigItem.html",
		
		//resources
		"Resources/Animation.Bottom.1.svg",
		"Resources/Animation.Bottom.2.svg",
		"Resources/Animation.Bottom.3.svg",
		"Resources/Animation.Top.1.svg",
		"Resources/Animation.Top.2.svg",
		"Resources/Animation.Top.3.svg",
		"Resources/Background.svg",
		"Resources/Icons.Back.svg",
		"Resources/Icons.List.svg",
		"Resources/Icons.Minus.svg",
		"Resources/Icons.Settings.svg"
	];
	
	this.private.totalDone = 0;
	
	var _this = this;
	setTimeout(function(){
		_this.private.loadNext.call(_this);
	}, _startupDelay_);
};

AppCacher.prototype.addObserver = function(callback){
	this.private.observers.push(callback);
};

AppCacher.prototype.private = {};

AppCacher.prototype.private.callObservers = function(){
	for (var c in this.private.observers)
	{
		var curr = this.private.observers[c];
		
		curr.apply(this, arguments);
	}
};

AppCacher.prototype.private.loadNext = function(){
	if (this.private.totalDone == this.private.objectsList.length)
	{
		this.private.callObservers.call(this, this.private.totalDone, this.private.totalDone, "");
		return;
	}
	
	var resource = this.private.objectsList[this.private.totalDone];
	this.private.callObservers.call(this, this.private.objectsList.length, this.private.totalDone, resource);
	
	SJL.cacheOrGet(resource, function(){
		this.private.totalDone++;
		
		this.private.loadNext.call(this);
		
	}, this);
};