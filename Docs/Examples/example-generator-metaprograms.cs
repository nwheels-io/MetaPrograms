//-------------------------------------------------------
// MetaPrograms generator - 26 lines
//-------------------------------------------------------

PUBLIC.CLASS("MyController", () => {
    EXTENDS<Controller>();
    ATTRIBUTE<RouteAttribute>("api/[controller]");

    PRIVATE.READONLY.FIELD<IService>("_service", out var @serviceField);
    PRIVATE.READONLY.FIELD<ILogger>("_logger", out var @loggerField);

    PUBLIC.CONSTRUCTOR(() => {
        PARAMETER<IService>("service", out var @service);
        PARAMETER<ILogger>("logger", out var @logger);
    });

    PUBLIC.FUNCTION<IActionResukt>("DoSomething", () => {
        ATTRIBUTE<HttPostAttribute>();
        PARAMETER<string>("value", out var @value, () => {
            ATTRIBUTE<FromBodyAttribute>();
        });

        DO.TRY(() => {
            @serviceField.DOT("DoSomething").INVOKE(@value);
            DO.RETURN(THIS.DOT("Ok").INVOKE());
        }).CATCH<Exception>(@e => {
            @loggerField.DOT("Error").INVOKE(INTERPOLATE(@e.DOT("GetType").DOT("Name"), " ", @e.DOT("Message"));
            DO.RETURN(THIS.DOT("NotFound").INVOKE());
        })
    });
});
