#ifndef HEADER_KRPC_CLIENT
#define HEADER_KRPC_CLIENT

#include "krpc/connection.hpp"
#include <boost/shared_ptr.hpp>

namespace krpc {

  class Client {
    boost::shared_ptr<Connection> rpc_connection;
    boost::shared_ptr<Connection> stream_connection;
  public:
    Client(const boost::shared_ptr<Connection>& rpc_connection,
           const boost::shared_ptr<Connection>& stream_connection);
    std::string invoke(const std::string& service, const std::string& procedure);
  };

}

#endif