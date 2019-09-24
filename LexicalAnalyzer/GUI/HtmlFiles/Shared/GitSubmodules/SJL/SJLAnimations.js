new (function () {
    SJL.extend("fadeIn", function (milisseconds, _onEnd_,_context_) {
        this.setProperty("style.opacity", 0);
        this.show();
        this.animate(0, 1, milisseconds, function (currValue) {
            this.setProperty("style.opacity", currValue);
        }, function(){if (typeof(_onEnd_) != "undefined") _onEnd_.call(_context_ || this);});

        return this;
    });

    SJL.extend("fadeOut", function (milisseconds, _onEnd_, _context_) {
        this.animate(1, 0, milisseconds, function (currValue) {
            this.setProperty("style.opacity", currValue);
        }, function(){this.hide(); if (typeof(_onEnd_) != "undefined") _onEnd_.call(_context_ || this);});

        return this;
    });

    SJL.extend("slide", function (startLeft, endLeft, milisseconds, _onEnd_, _context_) {
        this.animate(startLeft, endLeft, milisseconds, function (currValue) {
            this.setProperty("style.left", currValue + "px");
        }, function(){if (typeof(_onEnd_) != "undefined") _onEnd_.call(_context_ || this);});

        return this;
    });

    SJL.extend("slideSpeedUp", function (startLeft, endLeft, milisseconds, _onEnd_, _context_) {
        this.upAni(startLeft, endLeft, milisseconds, function (currValue) {
            this.setProperty("style.left", currValue + "px");
        }, function(){if (typeof(_onEnd_) != "undefined") _onEnd_.call(_context_ || this);});

        return this;
    });

    SJL.getRgbComponents = function(HexOrRgbCssColor){
        HexOrRgbCssColor = HexOrRgbCssColor.toUpperCase();
        if (HexOrRgbCssColor.indexOf('#') == 0){
            if (HexOrRgbCssColor.length == 4)
            {
                HexOrRgbCssColor = "#"+
                                    HexOrRgbCssColor[1] + HexOrRgbCssColor[1] + 
                                    HexOrRgbCssColor[2] + HexOrRgbCssColor[2] + 
                                    HexOrRgbCssColor[3] + HexOrRgbCssColor[3];
            }
    
            var r = parseInt("0x"+HexOrRgbCssColor.substr(1, 2));
            var g = parseInt("0x"+HexOrRgbCssColor.substr(3, 2));
            var b = parseInt("0x"+HexOrRgbCssColor.substr(5, 2));
            return {r: r, g:g, b:b, css: function(){return "rgb("+r+","+g+","+b+")"}};
    
        }
        else if (HexOrRgbCssColor.indexOf("RGB") == 0){
            HexOrRgbCssColor = HexOrRgbCssColor.split('(')[1].split(')')[0].split(',');
            var r = parseInt(HexOrRgbCssColor[0]);
            var g = parseInt(HexOrRgbCssColor[1]);
            var b = parseInt(HexOrRgbCssColor[2]);
            return {r: r, g:g, b:b, css: function(){return "rgb("+r+","+g+","+b+")"}};
        }
    };

    SJL.extend("animateColor", function(color1, color2, milisseconds, _cssProperty_, _onEnd_, _context_){
        _cssProperty_ = _cssProperty_ || "background";
        if (this.elements.length > 1)
        {
            var waitings = 0;
            this.do(function(currElement){
                waitings++;
                $(currElement).animateColor(color1, color2, milisseconds, _cssProperty_, function(){
                    waitings++;
                    if (waitings == 0)
                        _onEnd_.call(_context_);
                });
            });

            return
        }
        
        
        var orColor = SJL.getRgbComponents(color1);
        var destColor = SJL.getRgbComponents(color2);
        
        //get current color
        this.animate(0, 1, milisseconds, function(curr){
            //(DestColor - OrColor)+curr + OrColor
            var newColor_r =  parseInt((destColor.r - orColor.r) * curr + orColor.r);
            var newColor_g =  parseInt((destColor.g - orColor.g) * curr + orColor.g);
            var newColor_b =  parseInt((destColor.b - orColor.b) * curr + orColor.b);

            this.setCssProperty(_cssProperty_, "rgb("+newColor_r+","+newColor_g+","+newColor_b+")");
        }, _onEnd_, _context_);
    });

    SJL.extend("animateColor2", function(color1, color2, milisseconds, callback, _onEnd_, _context_){
        _context_ = _context_ || this;
        if (this.elements.length > 1)
        {
            var waitings = 0;
            this.do(function(currElement){
                waitings++;
                $(currElement).animateColor2(color1, color2, milisseconds, callback, function(){
                    waitings++;
                    if (waitings == 0)
                        _onEnd_.call(_context_);
                });
            });

            return
        }
        
        
        var orColor = SJL.getRgbComponents(color1);
        var destColor = SJL.getRgbComponents(color2);
        
        //get current color
        this.animate(0, 1, milisseconds, function(curr){
            //(DestColor - OrColor)+curr + OrColor
            var newColor_r =  parseInt((destColor.r - orColor.r) * curr + orColor.r);
            var newColor_g =  parseInt((destColor.g - orColor.g) * curr + orColor.g);
            var newColor_b =  parseInt((destColor.b - orColor.b) * curr + orColor.b);

            callback.call(_context_, "rgb("+newColor_r+","+newColor_g+","+newColor_b+")", destColor);
        }, _onEnd_, _context_);
    });
})();