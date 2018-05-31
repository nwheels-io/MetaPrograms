//--- code generators are stateless functions, which receive metadata of a programming model, and return corresponding code tree

    public static class WebApiControllerGenerator
    {
        public static TypeMember Controller(WebApiMetadata api) => 
            PUBLIC.CLASS($"{api.Interface.Name.TrimPrefix("I")}Controller", () => {
                EXTENDS<Controller>();
                ATTRIBUTE<RouteAttribute>("api/[controller]");

                PUBLIC.FUNCTION<string>("Index", () => {
                    ATTRIBUTE<HttpGetAttribute>();
                    DO.RETURN("Hello World!");
                });
            });
    }

//--- namespace

    NAMESPACE("My.Example", () => {
        // members of the namespace...
        // calling a generator here to generate a type will put the type inside this namespace
    });

    //---

    namespace My.Example
    {
        // members of the namespace...
    }

//--- class

    NAMESPACE("My.Example", () => {

        PUBLIC.CLASS("MyController", () => {
            // members of the class...
        })

    });

    //---

    namespace My.Example
    {
        public class MyController
        {
            // members of the class...
        }
    }

//--- substitute values with string interpolation

    NAMESPACE($"{ServiceMetadata.CompanyName}.{ServiceMetadata.ServiceName}.Api", () => {

        PUBLIC.CLASS($"{ApiMetadata.InterfaceType.Name.TrimPrefix("I")}Controller", () => {
            // members of the class...
        })

    });

    //---

    namespace GreatCompany.CoolService.Api
    {
        public class BillingApiController
        {
            // members of the class...
        }
    }

//--- specify base type and interfaces

    NAMESPACE("My.Example", () => {

        PUBLIC.CLASS("MyController", () => {
            EXTENDS<Controller>();
            IMPLEMENTS<IDisposable>();

            // members of the class...
        })

    });

    //---
    
    using System;  // necessarey usings are automatically added; no need to specify them in the generator
    using Microsoft.AspNetCore.Mvc;

    namespace My.Example
    {
        public class MyController : Controller, IDisposable
        {
            // members of the class...
        }
    }

//--- add attributes

    PUBLIC.CLASS("MyController", () => {
        ATTRIBUTE<RouteAttribute>("api/[controller]");
        ATTRIBUTE<EnableCorsAttribute>("DefaultPolicy");

        // members of the class...
    })

    //---

    [Route("api/[controller]")]
    [EnableCors("DefaultPolicy")]
    public class MyController
    {
        // members of the class...
    }

//--- add attributes with named arguments

    PUBLIC.CLASS("MyController", () => {
        ATTRIBUTE<RouteAttribute>("api/[controller]", () => {
            NAMED_ARG(nameof(RouteAttribute.Name), "ControllerRoute");
            NAMED_ARG(nameof(RouteAttribute.Order), 10);
        });

        // members of the class...
    })

    //---

    [Route("api/[controller]", Name = "ControllerRoute", Order = 10)]
    public class MyController
    {
        // members of the class...
    }

//--- add field

    PUBLIC.CLASS("MyController", () => {

        PRIVATE.READONLY.FIELD<string>("_value");

    });

    //---

    public class MyController
    {
        private readonly string _value;
    }

//--- apply attribute on a field

    PUBLIC.CLASS("MyController", () => {
        ATTRIBUTE<DataContractAttribute>();

        PRIVATE.READONLY.FIELD<string>("_value", () => {
            ATTRIBUTE<DataMemberAttribute>();
        });

    });

    //---

    [DataContract]
    public class MyController
    {
        [DataMember]
        private readonly string _value;
    }

//--- void method

    PUBLIC.CLASS("MyController", () => {

        PUBLIC.VOID("DoSomething", () => {

            // method body...

        });

    });

    //---

    public class MyController
    {
        public void DoSomething()
        {
            // method body...
        }
    }

//--- non-void method and returning a value

    PUBLIC.CLASS("MyController", () => {

        PUBLIC.FUNCTION<int>("GetSomething", () => {

            DO.RETURN(123);

        });

    });

    //---

    public class MyController
    {
        public int GetSomething()
        {
            return 123;
        }
    }

//--- method with parameters 

    PUBLIC.CLASS("MyController", () => {

        PUBLIC.VOID("AddPerson", () => {
            PARAMETER<string>("name");
            PARAMETER<int>("age");

            // do something...
        });

    });

    //---

    public class MyController
    {
        public void AddPerson(string name, int age)
        {
            // do something...
        }
    }

//--- method with attributes 

    PUBLIC.CLASS("MyController", () => {

        PUBLIC.FUNCTION<IActionResult>("GetPerson", () => {
            ATTRIBUTE<HttpGetAttribute>("person/{id}");
            PARAMETER<int>("id", out var @id);

            // do something...
        });

    });

    //---

    public class MyController
    {
        [HttpGet("person/{id}")]
        public IActionResult GetPerson(int id)
        {
            // do something...
        }
    }

//--- applying attributes to parameters

    PUBLIC.CLASS("MyController", () => {

        PUBLIC.FUNCTION<IActionResult>("GetPerson", () => {
            PARAMETER<int>("id", out var @id, () => {
                ATTRIBUTE<FromQueryAttribute>();
            });

            // do something...
        });

    });

    //---

    public class MyController
    {
        public IActionResult GetPerson([FromQuery] int id)
        {
            // do something...
        }
    }

//--- declaring and initializing local variables

    PUBLIC.CLASS("MyController", () => {

        PUBLIC.VOID("DoSomething", () => {
            LOCAL<int>("count");
            LOCAL<DayOfWeek>("day", DayOfWeek.Monday);

            // do something...
        });

    });

    //---

    public class MyController
    {
        public void DoSomething()
        {
            int count;
            var day = DayOfWeek.Monday;

            // do something...
        }
    }

//--- referencing locals, parameters, and fields; initialization; assignment

    PUBLIC.CLASS("MyController", () => {

        PRIVATE.FIELD<string>("_value", out var @valueField, initialValue: "");

        PUBLIC.FUNCTION<string>("SwapValue", () => {
            PARAMETER<string>("newValue", out var @newValue);

            LOCAL<string>("temp", out var @temp, initialValue: @valueField); 
            @valueField.ASSIGN(@newValue);
            DO.RETURN(@temp);
        });

    });

    //---

    public class MyController
    {
        private string _value = "";

        public void string SwapValue(string newValue)
        {
            var temp = _value;
            _value = newValue;
            return temp;
        }
    }

//--- accessing members with DOT(...); the "+" operator

    PUBLIC.VOID("DoSomething", () => {
        PARAMETER<Person>("person", out var @person);
        LOCAL<string>("fullName", out var @fullName); 

        @fullName.ASSIGN(@person.DOT("FirstName") + " " + @person.DOT("LastName"));

        // do something...
    });

    //---

    public void DoSomething(Person person)
    {
        string fullName;
        fullName = person.FirstName + " " + person.LastName;

        // do something...
    }

//--- basic operators; invoke a method

    PUBLIC.FUNCTION<string>("DoSomething", () => {
        PARAMETER<string>("str", out var @str);
        PARAMETER<int>("num", out var @num);

        DO.RETURN(@str + @num.DOT("ToString").INVOKE());
    });

    //---

    public string DoSomething(string str, int num)
    {
        return str + num.ToString();
    }

//--- logical operators; if statement; invoking method with arguments

    PUBLIC.FUNCTION<string>("TruncateIfLong", () => {
        PARAMETER<string>("str", out var @str);
        PARAMETER<int>("maxLength", out var @maxLength);

        DO.IF(@str != null && @str.DOT("Length") > @maxLength).THEN(() => {
            DO.RETURN(@str.DOt("Substring").INVOKE(0, @maxLength));
        }).ELSE(() => {
            DO.RETURN(@str);
        });
    });

    //---

    public string TruncateIfLong(string str, int maxLength)
    {
        if (str != null && str.Length > maxLength)
        {
            return str.Substring(0, maxLength);
        }
        else
        {
            return str;
        }
    }

//--- try/catch

    TRY(() => {

        @providerField.DOT("DoSomething").INVOKE();

    }).CATCH<FileNotFoundException>(@e => {
        
        @loggerField.DOT("Error").INVOKE(@e.DOT("GetType").DOT("Name") + " " + @e.DOT("Message"));

    }).FINALLY(() => {

        @providerField.DOT("EndSomething").INVOKE();

    });

    //---

    try
    {
        _provider.DoSomething();
    }
    catch (FileNotFoundException e)
    {
        _logger.Error(e.GetType().Name + ": " + e.Message);
    }
    finally
    {
        _provider.EndSomething();
    }

//-- string interpolation

    @loggerField.DOT("Error").INVOKE(INTERPOLATE(@e.DOT("GetType").DOT("Name"), " ", @e.DOT("Message"));

    //---

    _logger.Error($"{e.GetType().Name}: {e.Message}");

