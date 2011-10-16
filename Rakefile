
task :default => "all"

def csc(cmd)
  cmd = "csc /nologo #{cmd}"
  puts cmd
  system cmd
end

task :all => [
  :libraries , :client
] do end

task :libraries do
  csc '/t:library /out:bin/DynamicJson.dll lib/DynamicJson.cs'
  csc '/t:library /r:bin/DynamicJson.dll   /out:bin/Auth.dll BasyuraOrg.Twitter/Auth.cs'
  csc '/t:library /r:bin/DynamicJson.dll /r:bin/Auth.dll /out:bin/Twicseratops.dll BasyuraOrg.Twitter/Twicseratops.cs'
end

task :client do
#  csc '/r:Twicseratops.dll Client.cs'
end
