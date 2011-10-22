#!ir

require File.dirname(__FILE__) + '/../bin/Twicseratops.dll'
require 'rspec'

include System
include BasyuraOrg::Twitter

puts Twicseratops.NewRegister.GetAuthorizeUrl

t = Twicseratops.new("","")
puts t.FriendsTimeline
#puts t.Update(System::String.new "hi")


