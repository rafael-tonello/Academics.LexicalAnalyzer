_ServerLink= function(){
	//the websocket object pointer
	this._ws = null 
	
	/*the  list of general observers. When the connection with server is changed
	 * or  any  variable  is  changed  on  the  server,  these  observer will be 
	 * notified. Unlike  the  'variableObservers',  these  observer will receive 
	 * connection  information messages.*/
	this._generalObservers = [];
	
    this._startVariablesWebSocket.();
};

/* this method can be used to receive all events occured in the server 
(including receivevment of incoming variables) */
_ServerLink.prototype.observateMesages= function(func, _context_){
	_context_ || this;
	func.context = _context_;
	this._generalObservers.push(func);
};

/* the  method  bellow  is  called  by  the  constructor  and in socket error or 
 * disconnection cases. Its responsability is to connect to websocket and assign
 * the WebSocket events, parse data and call observers */
_ServerLink.prototype._startVariablesWebSocket = function(){
	var _this = this;
	this._callGnObservers.call(_this, "WsConnecting");
	this._ws= new WebSocket("ws://"+location.host);

	this._ws.onopen = function(e) {
		_this.connected = true;
		_this._callGnObservers.call(_this, "WsConnected", e);
		console.log("Websocket connected");
	};

	this._ws.onmessage = function(event) {
		//event.data contains the text message
		//messages are separated byy '|'
		var messageObj = JSON.parse(event.data);
        var message = message.message;
        var arguments = message.arguments;
        var id = message.id;
	
		try{
			_this._callGnObservers.call(_this, message, messageObj);
		}catch(e){
            console.error(e);
            if (messageObj.waitingResponse == true)
            {
                //send the error to server
            }
        }
	};

	this._ws.onclose = function(event) {
		_this.connected = false;
		
		_this._callGnObservers.call(_this, "WsClosed", event);
		_this._callGnObservers.call(_this, "WsDisconnected", event);
		
		//try reestablish the connection
		setTimeout(function(){
			_this._startVariablesWebSocket.call(_this);
		}, 1000);
	};
};

/* this private method is used by ServerLink to notify observer added by method
 * "observateAllEvents" */
_ServerLink.prototype._callGnObservers = function(message, args){
	//notifyObservers about the new message
	this._totalEvents++;
	for (var currObserverI in this._generalObservers) {
		var currObserver = this._generalObservers[currObserverI];
		try{
			currObserver.call(currObserver.context ? currObserver.context : this, message, args);
		}
		catch(e){console.error(e);}
	}
};

