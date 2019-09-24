SJL is javascript framework I use to make my SPA’s. I wrote it to understant more about Javascript and to make my work easier.

# 1) Resources overview
## 1.1) Selector

The first thing I need to tell to you about is the element selector of SJL. The selector uses  CSS selectors to get DOM elements 	and create an SJL instance to work with them. Once selected, you can do many things with the elements. 
To do  things with these elements, you must call methodos of returned object. Bellow these methods will be described in detail.
The selector function is ‘$’. 

See bellow an use example of the oselector:

```javascript
$(“.allElementsWithThisClass”)
```

### 1.1.1) SJL Methods to work with elements	
	
#### 1.1.1.1) clearObject
#### 1.1.1.2) hide
#### 1.1.1.3) show
#### 1.1.1.4) setValue
#### 1.1.1.5) getValue
#### 1.1.1.6) animate
#### 1.1.1.7) upSpeedAnimate
#### 1.1.1.8) downSpeedAnimate
#### 1.1.1.9) include
#### 1.1.1.10) includeUsingTags
#### 1.1.1.11) autoLoadComponents
#### 1.1.1.12) loadHtmlText
#### 1.1.1.13) loadHtml
#### 1.1.1.14) preloadHtml
#### 1.1.1.15) loadStaticComponent
#### 1.1.1.16) loadActivity
#### 1.1.1.17) callEvent
#### 1.1.1.18) setProperty
#### 1.1.1.19) setAttribute
#### 1.1.1.20) getProperty
#### 1.1.1.21) setCssProperty
#### 1.1.1.22) getCssProperty
#### 1.1.1.23) getComputedCssProperty
#### 1.1.1.24) download
#### 1.1.1.25) stringify
#### 1.1.1.26) bind
#### 1.1.1.27) bindAttribute

## includding css and javascript files

## filling elemnts with HTML files

## Loading components
SJL can load components by addresses in the elements in the DOM. When SJL loads a component, it perform many operations and creste many references in the created object. 
The first thing to be highlited is the instance of loaded component: When SJL loads an html file into a DOM element, its create an instance of the component class if this name is equals to filename witout the extension, eg: If the file “MyComponent.html” contains a class “MyComponent”, this class will be instantiated. Once time instantiated, the chosen DOM element will receive an reference to new object. This reference will be the same name of classe (on our example, the new property of the element will be called “MyComponent”).

#loops
##SJLForeach
	SJLForeach loop allows you to auto create html based in an Javascript array.
	For each element in the informed element array, the SJL will make a copy of 
	desired element.
	
``` html
<div SJLForeach="c in [1,2,3,4,5,6,7]">
	This div is the {{c}} in the div list
<div>
```

## Activities

## Auto loading activities by parsing the current URL

# using SJL as Framework

## Starting up SJL


# Requests 

## Requests with callbacks

## Requests with Promises

## get

## post

## delete

## cacheOrGet






----
----
# how to (faq)
## generic
### including another js and css files

## start activity auto loader
### enabling url monitor
	
### change url
#### changeUrl
		
#### go
	

## elements
### catch elements
	
### do anithing to a list of elements

To do actions with a list of elements uses the 'do' method. 'do' method is very similar to a 'foreach' loop control.

```javascript
$(".allElementsOfWithThisClass").do((currElement) =>{
    currElement.className = "otherClass";
});
```

### show
```javascript
$("#elementId").show();
```

### hide
```javascript
$("#elementId").hide();
```

### animate
```javascript
$(".slideTheseElements").animate(0, 100, (curr) =>{
    //inside this function, 'this' handles to '$(".slideTheseElements")' SJL instance

    this.setCssProperty("height", curr + "px");
})
```
		
#### linerar
	
#### upspeed
	
#### downspeed
		

### setValue

### getValue

### setting and html string inside a element

### loading external html inside elements

### loading a activity/component inside a element

#### SJLEnable - enabling or disable Css and Javascript parsing

### autoload components using SJLLoad

### autoload component using <SJL-folder.componentname

### using attributes

### create my own events
	
### setting Attributes and 'properties' of elements
	
### download of an element content

It's possible do downlod a elemnt content as a file. To do this, follow this example:

```javascript
$("#myElement").download("suggestFilename.html");

//if yout want, you can specify the mimetype
#("#myElement").download("suggest.txt", "text/plain");
```
	
	
## requests
SJL allow a easy way to load resources from the server using request functions. Each function can be used with callback functions or Promises to receive the  result data.

### making requests
		
#### get
To make an GET request, use the "get" function.

```javascript
    //using promises
    SJL.get("server/resource").then((result) => {
        console.log("result: ", result);
    });

    //using callback function
    SJL.get("server/resource", (result) => {
        console.log("result: ", result);
    });
``` 
		
#### post
post function is ver similar to another request methods, but receive a adicional parameter with the data to be sent to the server.

```javascript
    //using promises
    SJL.post("server/resource", "some data, like string or object").then((result) => {
        console.log("result: ", result);
    });

    //using callback function
    SJL.post("server/resource", "some data, like string or object", (result) => {
        console.log("result: ", result);
    });
``` 
		
#### delete
To delete some resource from server, just call 'delete' function:

```javascript
    //using promises
    SJL.delete("server/resource").then((result) => {
        console.log("result: ", result);
    });

    //using callback function
    SJL.delete("server/resource", (result) => {
        console.log("result: ", result);
    });
``` 
		
#### generic requests

## HTML Loops

#### Foreach inside a foreach
``` html
<div sjlforeach="a in [1, 2, 3, 4, 5, 6, 7, 8, 9]">
	childs of {{a}}:
	<div sjlforeach="b in [1, 2, 3, 4, 5, 6, 7, 8, 9]" style="padding-left:15px">
		Div {{a}}.{{b}}
	</div>
</div>
```