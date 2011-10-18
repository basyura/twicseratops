
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

def csc(config)
  cmd = "csc /nologo #{REF_ASSEMBLIES} "
  cmd << '/t:library ' unless config.key? :t

  config.each_pair do |key, value|
    key = :recurse if key == :src
    if value.kind_of?(Array)
      value.each {|v| cmd << "/#{key}:#{v.to_s.gsub('/','\\\\\\')} "}
    else
      cmd << "/#{key}:#{value.to_s.gsub('/' , '\\\\\\')} "
    end
  end


  puts "compile ... #{config[:src] || config[:recurse]}"
  if ARGV.length != 0 && ARGV[0] == 'verbose=true'
    puts cmd
  end

  out = `#{cmd}`
  if out != ""
    puts "\n  " + out
    exit
  end
end

# task

task :default => "all"

task :all => [
  :libraries , :client
] do end

task :libraries do
  csc out: 'bin/DynamicJson.dll'  , src: 'lib/DynamicJson.cs'
  csc out: 'bin/Auth.dll'         , src: 'src/BasyuraOrg.Twitter/Auth.cs'         , r: 'bin/DynamicJson.dll'
  csc out: 'bin/Twicseratops.dll' , src: 'src/BasyuraOrg.Twitter/Twicseratops.cs' , r: ['bin/DynamicJson.dll', 'bin/Auth.dll']
end

task :client do
  csc t: :exe, out: 'bin/Client.exe', recurse: 'sample/Client.cs', r: 'bin/Twicseratops.dll'
end
