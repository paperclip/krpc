#!/usr/bin/env python
# kRPC C++ client generator
# Author: Pranav Srinivas Kumar
# Date: 2015.05.19

import sys
import os.path
import argparse
from Cheetah.Template import Template
import krpc

parser = argparse.ArgumentParser(description='Generate C++ client code for kRPC')
parser.add_argument('--address', dest='address', default='127.0.0.1', action='store',
                    help='Server address (default: 127.0.0.1)')
parser.add_argument('--rpc-port', dest='rpc_port', default=50000, type=int, action='store',
                    help='RPC port (default: 50000)')
parser.add_argument('service', nargs='?', action='store',
                    help='Service name (default: all services)')
args = parser.parse_args()

class Parameter:
    def __init__(self):
        self.datatype = ''
        self.name = ''
        self.position = 0

    def generate_paramstring(self):
        if self.datatype == 'KRPC.Tuple':
            return 'double ' + self.name + '_x, ' + 'double ' + self.name + '_y, ' + 'double ' + self.name + '_z'
        elif self.datatype == 'KRPC.List':
            return 'std::vector<uint64_t> ' + self.name + '_vector'
        elif self.datatype == 'KRPC.Dictionary':
            return 'krpc::Dictionary ' + self.name + '_dict'
        elif self.datatype == 'KRPC.Request':
            self.name = 'input_request'
            return 'krpc::Request ' + self.name
        elif self.datatype == 'uint64' or self.datatype == 'uint32' or self.datatype == 'int32' or self.datatype == 'int64':
            return self.datatype + '_t ' + self.name
        elif self.datatype == 'string':
            return 'std::' + self.datatype + ' ' + self.name
        else:
            return self.datatype + ' ' + self.name

    def generate_requeststring(self):
        if self.datatype == 'KRPC.Tuple':
            return self.name + '_x, ' + self.name + '_y, ' + self.name + '_z'
        elif self.datatype == 'KRPC.List':
            return self.name + '_vector'
        elif self.datatype == 'KRPC.Dictionary':
            return self.name + '_dict'
        elif self.datatype == 'KRPC.Request':
            self.name = 'input_request'
            return self.name
        elif self.datatype == 'uint64' or self.datatype == 'uint32' or self.datatype == 'int32' or self.datatype == 'int64':
            return self.name
        elif self.datatype == 'string':
            return self.name
        else:
            return self.name

class Procedure:
    def __init__(self):
        self.name = ''
        self.parameter_count = 0
        self.parameters = []
        self.return_type = ''
        self.args = ''
        self.input_args = ''
        self.request_args = ''

    def generate_input_args(self):
        for parameter in self.parameters:
            self.args += parameter.generate_paramstring() + ', '
            self.input_args += parameter.generate_paramstring() + ', '
            self.request_args += parameter.generate_requeststring() + ', '
        self.input_args = self.input_args[:-2]
        self.request_args = self.request_args[:-2]

    def generate_output_args(self):
        if self.return_type == '':
            self.args = self.args[:-2]
        else:
            if self.return_type == 'KRPC.Tuple':
                self.args += 'double& x, double& y, double& z'
            elif self.return_type == 'KRPC.List':
                self.args += 'std::vector<uint64_t>& return_vector'
            elif self.return_type == 'KRPC.Dictionary':
                self.args += 'krpc::Dictionary& return_dict'
            elif self.return_type == 'KRPC.Status':
                self.args += 'krpc::Status& return_value'
            elif self.return_type == 'KRPC.Services':
                self.args += 'krpc::Services& return_value'
            elif self.return_type == 'uint64':
                self.args += 'uint64_t& return_value'
            elif self.return_type == 'uint32':
                self.args += 'uint32_t& return_value'
            elif self.return_type == 'int32':
                self.args += 'int32_t& return_value'
            elif self.return_type == 'int64':
                self.args += 'int64_t& return_value'
            elif self.return_type == 'string':
                self.args += 'std::string& return_value'
            else:
                self.args += self.return_type + '& return_value'

    def generate_argstring(self):
        self.generate_input_args()
        self.generate_output_args()

class Service:
    def __init__(self):
        self.name = ''
        self.procedures = []

conn = krpc.connect(name=sys.argv[0], address=args.address, rpc_port=args.rpc_port)
services = []
for service in conn.krpc.get_services().services:
    if args.service is not None and service.name != args.service:
        continue
    new_service = Service()
    new_service.name = service.name

    for procedure in service.procedures:
        new_procedure = Procedure()
        new_procedure.name = str(procedure.name)
        if procedure.return_type != None:
            new_procedure.return_type = str(procedure.return_type)
        if len(procedure.parameters) > 0:
            new_procedure.parameter_count = procedure.parameters
            for i,parameter in enumerate(procedure.parameters):
                new_parameter = Parameter()
                new_parameter.position = i
                new_parameter.datatype = str(parameter.type)
                new_parameter.name = parameter.name
                if new_parameter.name == 'this':
                    new_parameter.name = new_procedure.name.split('_')[0] + '_ID'
                if new_parameter.datatype == 'uint64':
                    new_parameter.datatype += '_t'
                new_procedure.parameters.append(new_parameter)
        new_procedure.generate_argstring()
        new_service.procedures.append(new_procedure)
    services.append(new_service)

hpp_namespace = {'hash_include': '#include', 'services': services}
template = Template(file='templates/krpci_hpp.tmpl', searchList=[hpp_namespace])
hpp_file = str(template)
if not os.path.exists('include'):
    os.makedirs('include')
with open(os.path.join('include', 'krpci.hpp'), 'w') as f:
    f.write(hpp_file)

cpp_namespace = {'hash_include': '#include', 'services': services}
template = Template(file='templates/krpci_cpp.tmpl', searchList=[cpp_namespace])
cpp_file = str(template)
if not os.path.exists('src'):
    os.makedirs('src')
with open(os.path.join('src', 'krpci_generated.cpp'), 'w') as f:
    f.write(cpp_file)
