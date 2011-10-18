
REF_ASSEMBLIES = [
  '/r:Accessibility.dll',
  '/r:Microsoft.CSharp.dll',
  '/r:System.Configuration.dll',
  '/r:System.Configuration.Install.dll',
  '/r:System.Core.dll',
  '/r:System.Data.dll',
  '/r:System.Data.DataSetExtensions.dll',
  '/r:System.Data.Linq.dll',
  '/r:System.Deployment.dll',
  '/r:System.DirectoryServices.dll',
  '/r:System.dll',
  '/r:System.Drawing.dll',
  '/r:System.EnterpriseServices.dll',
  '/r:System.Management.dll',
  '/r:System.Messaging.dll',
  '/r:System.Runtime.Remoting.dll',
  '/r:System.Runtime.Serialization.dll',
  '/r:System.Runtime.Serialization.Formatters.Soap.dll',
  '/r:System.Security.dll',
  '/r:System.ServiceModel.dll',
  '/r:System.ServiceProcess.dll',
  '/r:System.Transactions.dll',
  '/r:System.Xml.dll',
  '/r:System.Xml.Linq.dll',
].join(' ')


task :default => "all"

def csc(cmd)
  cmd = "csc /nologo #{REF_ASSEMBLIES} #{cmd}"
  puts cmd + "\n\n"
  system cmd
end

task :all => [
  :libraries , :client
] do end

task :libraries do
  csc '/t:library /out:bin/DynamicJson.dll /recurse:lib\\\\DynamicJson.cs'
  csc '/t:library /r:bin/DynamicJson.dll   /out:bin/Auth.dll /recurse:src\\\\BasyuraOrg.Twitter\\\\Auth.cs'
  csc '/t:library /r:bin/DynamicJson.dll /r:bin/Auth.dll /out:bin/Twicseratops.dll /recurse:src\\\\BasyuraOrg.Twitter\\\\Twicseratops.cs'
end

task :client do
  csc '/t:exe /r:bin/Twicseratops.dll /out:bin/Client.exe /recurse:sample\\\\Client.cs'
end
