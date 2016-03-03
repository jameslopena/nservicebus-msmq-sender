# nservicebus-msmq-sender
This class allows you to send an xml serialized message that can be handled by an NserviceBus application without referencing Nservicebus assemblies

Usage:
``` c#
var sender = new Sender("quename@hostname");
sender.Send(MyObject);

var sender = new Sender("myqueue@localhost");
sender.Send(MyObject);
```

Working with NServicebus 3,4,5
