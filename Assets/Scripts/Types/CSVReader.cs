﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// WIP
/// CLEAN UP CODE!
/// Reads CSV then spits out parsed data. 
/// Can import any GMNS data format, will support other formats soon
/// </summary>
namespace TrafficSim
{
    public class CSVReader
    {
        //public GameObject nodePrefab, linkPrefab, agentPrefab;
        //public int multiplier;

        /// <summary>
        /// Creates a Node Using CSV Parser
        /// </summary>
        public void CreateNodeCSV(Network network, string NodeFileName)
        {
            var lines = File.ReadAllLines(Application.dataPath + "/Data" + "/" + NodeFileName + ".csv");
            int num = 0;
            string[] names = new string[25];
            Vector3 offset = Vector3.zero;
            //Vector3 offset = new Vector3();
            foreach (var line in lines)
            {
                var values = line.Split(',');

                if (line == lines[0])
                {
                    names = values;
                }
                if (line != lines[0])
                {
                    
                    Node node = new Node();
                    for (int i = 0; i < values.Length; i++)
                    {
                        node.data = values;
                        //node.vals = new string[num + 1][];
                        //node.vals[num] = new string[values.Length];
                        //node.vals[num][i] = values[i];
                        //Debug.Log(node.vals[num][i]);
                    }


                    // set any relevant information.
                    node.names = names;

                    for (int i = 0; i < names.Length; i++)
                    {
                        if (node.names[i] == "node_id")
                        {
                            node.external_node_id = Convert.ToInt32(node.data[i]);
                            network.internal_node_seq_no_dict.Add(num, node.external_node_id);
                        }
                        if (node.names[i] == "x_coord")
                        {
                            //nodeObj.transform.position = new Vector3(float.Parse(Convert.ToString(node.data[i])) * multiplier + offset.x, 0, 0);
                            node.xcoord = float.Parse(Convert.ToString(node.data[i]));
                        }
                        if (node.names[i] == "y_coord")
                        {
                            //nodeObj.transform.position = new Vector3(nodeObj.transform.position.x, 0, float.Parse(Convert.ToString(node.data[i])) * multiplier + offset.z);
                            node.ycoord = float.Parse(Convert.ToString(node.data[i]));
                        }

                    }
                    //set origin position to zero.
                    if (line == lines[1])
                    {
                        offset = new Vector3(-node.xcoord, 0, -node.ycoord);
                        node.xcoord += offset.x;
                        node.ycoord += offset.y;
                    }

                    // set external node position to then have links to connect to
                    network.external_node_pos.Add(num, new Vector3(node.xcoord - offset.x, 0, node.ycoord - offset.y));

                    //Debug.Log(nodeList.Count);
                    node.node_seq_no = num;

                    network.node_list.Add(node);
                    num++;
                }


            }
        }
        /// <summary>
        /// Creates A Link using CSV Parser.
        /// </summary>
        public void CreateLinkCSV(Network network, string LinkFileName)
        {
            var lines = File.ReadAllLines(Application.dataPath + "/Data" + "/" + LinkFileName + ".csv");
            int num = 0;
            string[] names = new string[25];
            foreach (var line in lines)
            {
                var values = line.Split(',');

                if (line == lines[0])
                {
                    names = values;
                }
                if (line != lines[0])
                {

                    Link link = new Link();
                    for (int i = 0; i < values.Length; i++)
                    {
                        link.data = values;
                        //node.vals = new string[num + 1][];
                        //node.vals[num] = new string[values.Length];
                        //node.vals[num][i] = values[i];
                        //Debug.Log(node.vals[num][i]);
                    }

                    // set any relevant information.
                    link.names = names;

                    for (int i = 0; i < names.Length; i++)
                    {
                        if (link.names[i] == "from_node_id")
                        {
                            link.external_from_node = int.Parse(Convert.ToString(link.data[i]));

                            link.from_node_seq_no = network.internal_node_seq_no_dict.FirstOrDefault(x => x.Value == link.external_from_node).Key;
                            Vector3 pos = Vector3.zero;
                            bool hasValue = network.external_node_pos.TryGetValue(link.from_node_seq_no, out pos);
                            if (hasValue)
                            {
                                //LinkObj.GetComponent<LineRenderer>().SetPosition(0, pos);
                            }
                            else
                            {
                                Console.WriteLine("Key not present");
                            }


                        }
                        if (link.names[i] == "to_node_id")
                        {
                            link.external_to_node = int.Parse(Convert.ToString(link.data[i]));

                            link.to_node_seq_no = network.internal_node_seq_no_dict.FirstOrDefault(x => x.Value == link.external_to_node).Key;

                            // declare pos
                            Vector3 pos = Vector3.zero;
                            bool hasValue = network.external_node_pos.TryGetValue(link.to_node_seq_no, out pos);
                            if (hasValue)
                            {
                                //LinkObj.GetComponent<LineRenderer>().SetPosition(1, pos);
                            }
                            else
                            {
                                Console.WriteLine("Key not present");
                            }
                        }
                        if (link.names[i] == "length")
                        {
                            link.length = float.Parse(Convert.ToString(link.data[i]));
                        }
                        if (link.names[i] == "lanes")
                        {
                            link.lanes = float.Parse(Convert.ToString(link.data[i]));
                        }
                        if (link.names[i] == "free_speed")
                        {
                            link.free_speed = float.Parse(Convert.ToString(link.data[i]));
                        }
                        if (link.names[i] == "capacity")
                        {
                            link.link_capacity = float.Parse(Convert.ToString(link.data[i])) * Convert.ToInt32(link.lanes);
                        }
                        if (link.names[i] == "link_type")
                        {
                            link.type = int.Parse(Convert.ToString(link.data[i]));
                        }
                        if (link.names[i] == "BPR_alpha1")
                        {
                            link.VDF_alpha = float.Parse(Convert.ToString(link.data[i]));
                        }
                        if (link.names[i] == "BPR_beta1")
                        {
                            link.VDF_alpha = float.Parse(Convert.ToString(link.data[i]));
                        }
                    }
                    //set info not related to csv file
                    link.link_seq_no = num;

                    // assign link's manager to manager
                    link.network = network;
                    //Debug.Log(offset);
                    network.link_list.Add(link);
                    num++;
                }


            }
        }
        public void CreateAgentCSV(Network network, string AgentFileName)
        {
            var lines = File.ReadAllLines(Application.dataPath + "/Data" + "/" + AgentFileName + ".csv");
            int num = 0;
            string[] names = new string[25];
            foreach (var line in lines)
            {
                var values = line.Split(',');

                if (line == lines[0])
                {
                    names = values;
                }
                if (line != lines[0])
                {
                    Agent agent = new Agent();
                    for (int i = 0; i < values.Length; i++)
                    {
                        agent.data = values;
                        //node.vals = new string[num + 1][];
                        //node.vals[num] = new string[values.Length];
                        //node.vals[num][i] = values[i];
                        //Debug.Log(node.vals[num][i]);
                    }

                    // set any relevant information.
                    agent.names = names;

                    for (int i = 0; i < names.Length; i++)
                    {
                        if (agent.names[i] == "agent_type")
                        {
                            agent.agent_type = Convert.ToString(agent.data[i]);
                        }
                        if (agent.names[i] == "o_node_id")
                        {
                            agent.o_node_id = Convert.ToInt32(agent.data[i]);
                        }
                        if (agent.names[i] == "d_node_id")
                        {
                            agent.o_node_id = Convert.ToInt32(agent.data[i]);
                        }
                        if (agent.names[i] == "d_node_id")
                        {
                            agent.departure_time_in_min = Convert.ToDouble(agent.data[i]);
                        }
                        if (agent.names[i] == "PCE")
                        {
                            agent.PCE = Convert.ToInt32(agent.data[i]);
                        }
                        if (agent.names[i] == "path_fixed_flag")
                        {
                            agent.path_fixed_flag = Convert.ToInt32(agent.data[i]);
                        }
                        if (agent.names[i] == "node_sequence")
                        {
                            var vals2 = Convert.ToString(agent.data[i]).Split(';');
                            for (int k = 0; k < vals2.Length; k++)
                            {
                                //agent.path_node_seq.Add(Convert.ToInt32(vals2[k]));
                            }
                        }
                    }
                    //set info not related to csv file
                    agent.agent_seq_no = num;
                    agent.agent_id = num;
                    // assign link's manager to manager
                    agent.network = network;
                    //Debug.Log(offset);
                    network.agent_list.Add(agent);
                    num++;
                }
                network.agent_list.Sort((a, b) => a.departure_time_in_min.CompareTo(b.departure_time_in_min));
            }
            Debug.Log("number of agents" + network.agent_list.Count);
        }
    }
}
