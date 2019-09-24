new (function (){
    /** Create a watch observer info container 
     * @param {string} varName - The name of the variable that must be observed
     * @param {Function} callback - The callback to be called when the variable is changed
     * @param {object} context - The context when callback will be called
     * @param {object} _args_ - Optional value that contais any argument to be passed to callback when the variable is changed
    */
    function Watch(varName, callback, context, _args_)
    {
        this.name = varName;
        this.callback = callback;
        this.context = context || window;
        this.args = _args_ || null;
        this.oldValue = "----invalidValue----"
    }

    this.watches = [];

    this.linkAttributes = function(el)
    {
        //scrolls throught the attributes for element
        for (var c = 0; c < el.attributes.length; c++)
        {
            var curr = el.attributes[c].name;
            //checks if the attribute contains one or more patterns of link
            if (el.getAttribute(curr) != null) 
            {
                if (el.getAttribute(curr).indexOf("{*") > -1)
                {
                    //take the original value or current attribute
                    var originalValue = el.getAttribute(curr);
                    //the variable bellow is used to find all variables in the originalValue
                    var remainValue = originalValue;
                    //the identified variables names will be placed in the array bellow 
                    var variableNames = [];

                    while (remainValue.indexOf("{*") > -1)
                    {
                        //get the variable name between '{*' and  '*}'
                        var variableName = remainValue.substring(remainValue.indexOf("{*")+2, remainValue.indexOf("*}"));
                        //add the variable to variables vector
                        variableNames.push(variableName);
                        //remove the found variable from remainValue
                        remainValue = remainValue.substring(remainValue.indexOf("*}")+2);

                        //try to create the variable, if not exists
                        try{
                            eval ("if (typeof("+variableName+") == 'undefined'){"+variableName+" = '';}");
                        }catch(e){}
                    }

                    //The Javascript treats the "Value 'property'" and "Value 'attribute'" independently.  The attribute "Value" is used to set property "Value" only on
                    //first page load. So it is necessary to make a separate treatment "Value" property
                    if (curr != "value")
                    {
                        //Observates each found variables
                        for (var contNames =0; contNames < variableNames.length; contNames++)
                        {
                            this.watches.push(new Watch(variableNames[contNames], function(newValue, args){
                                var value = args.originalValue;
                                for (var contNames2 = 0; contNames2 < args.variableNames.length; contNames2++)
                                {
                                    //try{
                                        eval("value = value.replace(/\\{\\*"+args.variableNames[contNames2]+"\\*\\}/g, "+args.variableNames[contNames2]+");");
                                    //}
                                    //catch(e){}
                                }

                                //change the component properties
                                args.el.setAttribute(args.curr, value + '');



                            }, this, {originalValue: originalValue, variableNames: variableNames, el: el, curr:curr}));
                        }
                    }
                    else
                    {
                        //when the curr is a value, just the first variable name is considered
                        this.watches.push(new Watch(variableNames[0], function(newValue){
                            el.value = newValue + '';
                        }, this, {originalValue: originalValue, variableNames: variableNames, el: el, curr:curr}));
                        
                        //It is also necessary to observe the "Value" property of the element. To do it, is necessary the definition of the a global name to element
                        var gloabalElName = "window."+SJL.getId();
                        eval (gloabalElName + "=el");
                        this.watches.push(new Watch(gloabalElName + ".value", function(newValue, args){
                            eval (arga.variableNames[0] + " = newValue +'';");
                        }, this, {originalValue: originalValue, variableNames: variableNames, el: el, curr:curr}));

                    }
                }
            }
        }
    };

    this.linkEvents = function(el)
    {

    };

    this.linkInnerText = function(el)
    {

    };

    this.linkStyles = function(el)
    {

    };

    this.monitorVariables = function()
    {
        for (var c in this.watches)
        {
            var changed = false;
            var newValue = "";
            try{
                eval ("changed = this.watches[c].oldValue != "+this.watches[c].name+";"+ 
                    "newValue = "+this.watches[c].name+";"+
                    "this.watches[c].oldValue = "+this.watches[c].name+";"
                );
            
            
                if (changed){
                    this.watches[c].callback.call(this.watches[c].context, newValue, this.watches[c].args);
                }
            }catch(e){ }
        }
        setTimeout(function(_this){_this.monitorVariables();}, 10, this);
    };
    


    this.start = function()
    {
        if (document.readyState != "complete")
        {
            setTimeout(function(_this){_this.start()}, 10, this);
            return;
        }

        
        SJL.extend("_linkDomWorker", this);
        SJL.extend(["linkDom", "bindDom"], function () {
            for (c in this.elements)
            {
                var curr = this.elements[c];
                this._linkDomWorkder.linkAttributes(curr);
                this._linkDomWorkder.linkEvents(curr);
                this._linkDomWorkder.linkInnerText(curr);
                this._linkDomWorkder.linkStyles(curr);
            }

            return this;
        });
        $("*").linkDom();
        this.monitorVariables();
    };
    this.start();
})();
